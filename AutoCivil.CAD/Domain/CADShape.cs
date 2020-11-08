using AutoCivil.CAD.Constants;
using netDxf.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoCivil.CAD.Domain
{
    public class CADShape
    {
        private CADShape _leftShape;
        private CADShape _rightShape;
        private CADShape _topShape;
        private CADShape _bottomShape;

        public bool Calculated { get; set; }
        public bool SearchOtherPlaces { get; set; }
        public bool Origin { get; set; }
        public bool Drawn { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }
        public int Width { get; set; }        
        public int Height { get; set; }
        public int ExternalX { get { return X - LeftWall; } }
        public int ExternalY { get { return Y + TopWall; } }
        public int ExternalWidth { get { return Width + LeftWall + RightWall; } }
        public int ExternalHeight { get { return Height + TopWall + BottomWall; } }
        public CADPlace Place { get; set; }
        public CADShape LeftShape { get { return _leftShape; } set { _leftShape = value; value._rightShape = this; } }
        public CADShape RightShape { get { return _rightShape; } set { _rightShape = value; value._leftShape = this; } }
        public CADShape TopShape { get { return _topShape; } set { _topShape = value; value._bottomShape = this; } }
        public CADShape BottomShape { get { return _bottomShape; } set { _bottomShape = value; value._topShape = this; } }
        public int TopWall { get; set; }
        public int BottomWall { get; set; }
        public int LeftWall { get; set; }
        public int RightWall { get; set; }

        public CADShape(CADPlace place)
        {
            TopWall = BottomWall = LeftWall = RightWall = Measures.EXTERNAL_WALL_THICKNESS;
            X = 0;
            Y = 0;
            Place = place;
            place.Shapes.Add(this);
            Origin = false;
            Drawn = false;
        }

        public void Locate(int x, int y, List<CADShape> shapes, CADShape initialShape)
        {
            this.X = x;
            this.Y = y;
            this.X2 = x + Width;
            this.Y2 = Y - Height;
            this.Calculated = true;
                                    
            if(this.RightShape != null && this.RightShape.Place == Place && !this.RightShape.Calculated)
            {
                var nextX = this.X + this.Width + (this.Place != this.RightShape.Place ? this.RightWall : 0);
                var nextY = this.Y - (this.RightShape.TopShape != null ? this.RightShape.TopWall : 0);
                this.RightShape.Locate(nextX, nextY, shapes, initialShape);
            }

            if (this.LeftShape != null && this.LeftShape.Place == Place && !this.LeftShape.Calculated)
            {
                var nextX = this.X - this.LeftShape.Width - (this.Place != this.LeftShape.Place ? this.LeftWall : 0);
                var nextY = this.Y;
                this.LeftShape.Locate(nextX, nextY, shapes, initialShape);
            }

            if (this.BottomShape != null && this.BottomShape.Place == Place && !this.BottomShape.Calculated)
            {
                var nextX = this.X;
                var nextY = this.Y - this.Height - (this.Place != this.BottomShape.Place ? this.BottomWall : 0);
                this.BottomShape.Locate(nextX, nextY, shapes, initialShape);
            }

            if (this.TopShape != null && this.TopShape.Place == Place && !this.TopShape.Calculated)
            {
                var nextX = this.X;
                var nextY = this.Y + this.Height + (this.Place != this.TopShape.Place ? this.TopWall : 0);
                this.TopShape.Locate(nextX, nextY, shapes, initialShape);
            }

            if(initialShape == this)
                this.LocateOtherPlaces(x, y, shapes, this);
        }

        public void LocateOtherPlaces(int x, int y, List<CADShape> shapes, CADShape initialShape)
        {
            this.SearchOtherPlaces = true;

            if (this.RightShape != null && this.RightShape.Place != Place && !this.RightShape.Calculated)
            {
                var nextX = this.X + this.Width + (this.Place != this.RightShape.Place ? this.RightWall : 0);
                var nextY = this.Y - (this.RightShape.TopShape != null ? this.RightShape.TopWall : 0);

                this.RightShape.Locate(nextX, nextY, shapes, this.RightShape);
            }

            if (this.LeftShape != null && this.LeftShape.Place != Place && !this.LeftShape.Calculated)
            {
                var nextX = this.X - this.LeftShape.Width - (this.Place != this.LeftShape.Place ? this.LeftWall : 0);
                var nextY = this.Y;
                this.LeftShape.Locate(nextX, nextY, shapes, this.LeftShape);
            }

            if (this.BottomShape != null && this.BottomShape.Place != Place && !this.BottomShape.Calculated)
            {
                var nextX = this.X;
                var nextY = this.Y - this.Height - (this.Place != this.BottomShape.Place ? this.BottomWall : 0);

                var leftShape = shapes.Where(s => s.Calculated && s.X2 >= nextX - this.RightWall && s.Y >= nextY && s.Y2 < nextY).FirstOrDefault();
                if (leftShape != null)
                    nextX += leftShape.RightWall;

                this.BottomShape.Locate(nextX, nextY, shapes, this.BottomShape);
            }

            if (this.TopShape != null && this.TopShape.Place != Place && !this.TopShape.Calculated)
            {
                var nextX = this.X;
                var nextY = this.Y + this.Height + (this.Place != this.TopShape.Place ? this.TopWall : 0);
                this.TopShape.Locate(nextX, nextY, shapes, this.TopShape);
            }

            //Procurando novos comodos 

            if (this.RightShape != null && this.RightShape.Place == Place && !this.RightShape.SearchOtherPlaces)
            {
                var nextX = this.X + this.Width + (this.Place != this.RightShape.Place ? this.RightWall : 0);
                var nextY = this.Y - (this.RightShape.TopShape != null ? this.RightShape.TopWall : 0);
                this.RightShape.LocateOtherPlaces(nextX, nextY, shapes, initialShape);
            }

            if (this.LeftShape != null && this.LeftShape.Place == Place && !this.LeftShape.SearchOtherPlaces)
            {
                var nextX = this.X - this.LeftShape.Width - (this.Place != this.LeftShape.Place ? this.LeftWall : 0);
                var nextY = this.Y;
                this.LeftShape.LocateOtherPlaces(nextX, nextY, shapes, initialShape);
            }

            if (this.BottomShape != null && this.BottomShape.Place == Place && !this.BottomShape.SearchOtherPlaces)
            {
                var nextX = this.X;
                var nextY = this.Y - this.Height - (this.Place != this.BottomShape.Place ? this.BottomWall : 0);
                this.BottomShape.LocateOtherPlaces(nextX, nextY, shapes, initialShape);
            }

            if (this.TopShape != null && this.TopShape.Place == Place && !this.TopShape.SearchOtherPlaces)
            {
                var nextX = this.X;
                var nextY = this.Y + this.Height + (this.Place != this.TopShape.Place ? this.TopWall : 0);
                this.TopShape.LocateOtherPlaces(nextX, nextY, shapes, initialShape);
            }
        }
    }
}
