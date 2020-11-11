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
            DrawToRight(shape.X, shape.Y, shape, place.Shapes, dxf);
        }

        private void DrawToRight(int x, int y, CADShape shape, List<CADShape> shapes, DxfDocument dxf)
        {
            var line = new Line(new Vector2(x, y), new Vector2(shape.X2, shape.Y)); //Desenhando o topo para direita
            line.Layer = _layerParede;
            dxf.AddEntity(line);

            //Tentando ir para cima
            var topShape = shapes.FirstOrDefault(s => s.X == shape.X2 && s.Y2 == y);

            if(topShape != null)
            {
                DrawToTop(shape.X2, shape.Y, topShape, shapes, dxf);
                return;
            }

            //Tentando ir para direita
            var rightShape = shapes.FirstOrDefault(s => s.X == shape.X2 && s.Y == y);
            if(rightShape != null)
            {
                DrawToRight(shape.X2, shape.Y, rightShape, shapes, dxf);
                return;
            }

            DrawToBottom(shape.X2, shape.Y, shape, shapes, dxf);
        }

        private void DrawToTop(int x, int y, CADShape shape, List<CADShape> shapes, DxfDocument dxf)
        {
            var line = new Line(new Vector2(x, y), new Vector2(shape.X, shape.Y)); //Desenhando lado esquerdo para cima
            line.Layer = _layerParede;
            dxf.AddEntity(line);

            if (shape == shape.Place.OriginShape) //Terminou de desenhar todo o comodo
                return;

            //Tentando ir para esquerda
            var leftShape = shapes.FirstOrDefault(s => s.X2 == x && s.Y2 == y);

            if (leftShape != null)
            {
                DrawToLeft(shape.X, shape.Y, leftShape, shapes, dxf);
                return;
            }

            //Tentando ir para direita
            var topShape = shapes.FirstOrDefault(s => s.X == x && s.Y2 == y);
            if (topShape != null)
            {
                DrawToTop(shape.X, shape.Y, topShape, shapes, dxf);
                return;
            }

            DrawToRight(shape.X, shape.Y, shape, shapes, dxf);
        }

        private void DrawToBottom(int x, int y, CADShape shape, List<CADShape> shapes, DxfDocument dxf)
        {
            var line = new Line(new Vector2(x, y), new Vector2(shape.X2, shape.Y2)); //Desenhando o lado direito para baixo
            line.Layer = _layerParede;
            dxf.AddEntity(line);

            //Tentando ir para direita
            var rightShape = shapes.FirstOrDefault(s => s.X <= shape.X2 && s.X2 > shape.X2 && s.Y == shape.Y2);

            if (rightShape != null)
            {
                DrawToRight(shape.X2, shape.Y2, rightShape, shapes, dxf);
                return;
            }

            //Tentando ir para baixo
            var bottomShape = shapes.FirstOrDefault(s => s.X2 == shape.X2 && s.Y == shape.Y2);
            if (bottomShape != null)
            {
                DrawToBottom(shape.X2, shape.Y2, bottomShape, shapes, dxf);
                return;
            }

            DrawToLeft(shape.X2, shape.Y2, shape, shapes, dxf);
        }

        private void DrawToLeft(int x, int y, CADShape shape, List<CADShape> shapes, DxfDocument dxf)
        {
            var line = new Line(new Vector2(x, y), new Vector2(shape.X, shape.Y2)); //Desenhando o lado de baixo para esquerda
            line.Layer = _layerParede;
            dxf.AddEntity(line);

            //Tentando ir para baixo
            var bottomShape = shapes.FirstOrDefault(s => s.X2 == shape.X && s.Y >= shape.Y2 && s.Y2 < shape.Y2);

            if (bottomShape != null)
            {
                DrawToBottom(shape.X, shape.Y2, bottomShape, shapes, dxf);
                return;
            }

            //Tentando ir para esquerda
            var leftShape = shapes.FirstOrDefault(s => s.X2 == x && s.Y2 == y);
            if (leftShape != null)
            {
                DrawToLeft(shape.X, shape.Y2, leftShape, shapes, dxf);
                return;
            }

            DrawToTop(shape.X, shape.Y2, shape, shapes, dxf);
        }
    }
}
