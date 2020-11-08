using AutoCivil.CAD.Domain;
using netDxf;
using netDxf.Collections;
using netDxf.Entities;
using netDxf.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCivil.CAD.Libraries
{
    public class Designer
    {
        private List<CADShape> ShapesDrawn = new List<CADShape>();
        private Layer _layerParede = new Layer("Paredes");
        public Designer()
        {
            _layerParede.Color = AciColor.Blue;
        }

        public void DesignCAD(string filename, List<CADShape> shapes, List<CADPlace> places)
        {
            DxfDocument dxf = new DxfDocument();

            ShapesDrawn = new List<CADShape>();

            shapes = shapes.OrderBy(s => s.X).ThenByDescending(s => s.Y).ToList();

            var shape = shapes.First();

            //setando as coordenadas X,Y de cada Shape
            shape.Origin = true;

            shape.Locate(0, 0, shapes, shape);
            DrawPlaces(places, dxf);
            //DrawExternalWall(shapes, dxf);
            //DrawShape(initialShape, dxf);            

            dxf.Save(filename);
        }

        private void DrawPlaces(List<CADPlace> places, DxfDocument dxf)
        {
            foreach (var place in places)
            {
                DrawPlace(place, dxf);
            }
        }

        private void DrawPlace(CADPlace place, DxfDocument dxf)
        {
            var shape = place.Shapes.OrderBy(s => s.X).ThenByDescending(s => s.Y).First();
            place.OriginShape = shape;
            DrawToRight(shape, place.Shapes, dxf);
        }

        private void DrawToRight(CADShape shape, List<CADShape> shapes, DxfDocument dxf)
        {
            var line = new Line(new Vector2(shape.X, shape.Y), new Vector2(shape.X2, shape.Y)); //Desenhando o topo para direita
            line.Layer = _layerParede;
            dxf.AddEntity(line);

            //Tentando ir para cima
            var topShape = shapes.FirstOrDefault(s => s.X == shape.X2 && s.Y2 == shape.Y);

            if(topShape != null)
            {
                DrawToTop(topShape, shapes, dxf);
                return;
            }

            //Tentando ir para direita
            var rightShape = shapes.FirstOrDefault(s => s.X == shape.X2 && s.Y == shape.Y);
            if(rightShape != null)
            {
                DrawToRight(rightShape, shapes, dxf);
                return;
            }

            DrawToBottom(shape, shapes, dxf);
        }

        private void DrawToTop(CADShape shape, List<CADShape> shapes, DxfDocument dxf)
        {
            var line = new Line(new Vector2(shape.X, shape.Y2), new Vector2(shape.X, shape.Y)); //Desenhando lado esquerdo para cima
            line.Layer = _layerParede;
            dxf.AddEntity(line);

            if (shape == shape.Place.OriginShape) //Terminou de desenhar todo o comodo
                return;

            //Tentando ir para esquerda
            var leftShape = shapes.FirstOrDefault(s => s.X2 == shape.X && s.Y2 == shape.Y);

            if (leftShape != null)
            {
                DrawToLeft(leftShape, shapes, dxf);
                return;
            }

            //Tentando ir para direita
            var topShape = shapes.FirstOrDefault(s => s.X == shape.X && s.Y2 == shape.Y);
            if (topShape != null)
            {
                DrawToTop(topShape, shapes, dxf);
                return;
            }

            DrawToRight(shape, shapes, dxf);
        }

        private void DrawToBottom(CADShape shape, List<CADShape> shapes, DxfDocument dxf)
        {
            var line = new Line(new Vector2(shape.X2, shape.Y), new Vector2(shape.X2, shape.Y2)); //Desenhando o lado direito para baixo
            line.Layer = _layerParede;
            dxf.AddEntity(line);

            //Tentando ir para direita
            var rightShape = shapes.FirstOrDefault(s => s.X == shape.X2 && s.Y == shape.Y2);

            if (rightShape != null)
            {
                DrawToRight(rightShape, shapes, dxf);
                return;
            }

            //Tentando ir para baixo
            var bottomShape = shapes.FirstOrDefault(s => s.X2 == shape.X2 && s.Y == shape.Y2);
            if (bottomShape != null)
            {
                DrawToBottom(bottomShape, shapes, dxf);
                return;
            }

            DrawToLeft(shape, shapes, dxf);
        }

        private void DrawToLeft(CADShape shape, List<CADShape> shapes, DxfDocument dxf)
        {
            var line = new Line(new Vector2(shape.X2, shape.Y2), new Vector2(shape.X, shape.Y2)); //Desenhando o lado de baixo para esquerda
            line.Layer = _layerParede;
            dxf.AddEntity(line);

            //Tentando ir para baixo
            var bottomShape = shapes.FirstOrDefault(s => s.X2 == shape.X && s.Y == shape.Y2);

            if (bottomShape != null)
            {
                DrawToBottom(bottomShape, shapes, dxf);
                return;
            }

            //Tentando ir para esquerda
            var leftShape = shapes.FirstOrDefault(s => s.X2 == shape.X && s.Y == shape.Y);
            if (leftShape != null)
            {
                DrawToLeft(leftShape, shapes, dxf);
                return;
            }

            DrawToTop(shape, shapes, dxf);
        }

        private void DrawShape(CADShape shape, DxfDocument dxf)
        {
            if (ShapesDrawn.Contains(shape))
                return;

            ShapesDrawn.Add(shape);

            DrawRectangleShape(shape, dxf);
            DrawWallShape(shape, dxf);

            if (shape.RightShape != null)
            {
                DrawShape(shape.RightShape, dxf);
            }

            if (shape.BottomShape != null)
            {
                DrawShape(shape.BottomShape, dxf);
            }
        }

        private void DrawRectangleShape(CADShape cadShape, DxfDocument dxf)
        {
            Line line = null;
            
            if(cadShape.TopShape == null || cadShape.TopShape.Place != cadShape.Place)
            {
                line = new Line(new Vector2(cadShape.X, cadShape.Y), new Vector2(cadShape.X + cadShape.Width, cadShape.Y));
                line.Layer = _layerParede;
                dxf.AddEntity(line);
            }

            if (cadShape.RightShape == null || cadShape.RightShape.Place != cadShape.Place)
            {
                line = new Line(new Vector2(cadShape.X + cadShape.Width, cadShape.Y), new Vector2(cadShape.X + cadShape.Width, cadShape.Y - cadShape.Height));
                line.Layer = _layerParede;
                dxf.AddEntity(line);
            }

            if (cadShape.BottomShape == null || cadShape.BottomShape.Place != cadShape.Place)
            {
                line = new Line(new Vector2(cadShape.X + cadShape.Width, cadShape.Y - cadShape.Height), new Vector2(cadShape.X, cadShape.Y - cadShape.Height));
                line.Layer = _layerParede;
                dxf.AddEntity(line);
            }

            if (cadShape.LeftShape == null || cadShape.LeftShape.Place != cadShape.Place)
            {
                line = new Line(new Vector2(cadShape.X, cadShape.Y - cadShape.Height), new Vector2(cadShape.X, cadShape.Y));
                line.Layer = _layerParede;
                dxf.AddEntity(line);
            }                           
        }

        private void DrawWallShape(CADShape cadShape, DxfDocument dxf)
        {
            Line line = null;

            if (cadShape.TopShape == null)
            {
                line = new Line(new Vector2(cadShape.ExternalX, cadShape.ExternalY), new Vector2(cadShape.ExternalX + cadShape.ExternalWidth, cadShape.ExternalY));
                line.Layer = _layerParede;
                dxf.AddEntity(line);
            }

            if (cadShape.RightShape == null)
            {
                line = new Line(new Vector2(cadShape.ExternalX + cadShape.ExternalWidth, cadShape.ExternalY), new Vector2(cadShape.ExternalX + cadShape.ExternalWidth, cadShape.ExternalY - cadShape.ExternalHeight));
                line.Layer = _layerParede;
                dxf.AddEntity(line);
            }

            if (cadShape.BottomShape == null)
            {
                line = new Line(new Vector2(cadShape.ExternalX + cadShape.ExternalWidth, cadShape.ExternalY - cadShape.ExternalHeight), new Vector2(cadShape.ExternalX, cadShape.ExternalY - cadShape.ExternalHeight));
                line.Layer = _layerParede;
                dxf.AddEntity(line);
            }

            if (cadShape.LeftShape == null)
            {
                line = new Line(new Vector2(cadShape.ExternalX, cadShape.ExternalY - cadShape.ExternalHeight), new Vector2(cadShape.ExternalX, cadShape.ExternalY));
                line.Layer = _layerParede;
                dxf.AddEntity(line);
            }            
        }
    }
}
