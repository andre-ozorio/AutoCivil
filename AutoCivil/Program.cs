using AutoCivil.CAD.Domain;
using AutoCivil.CAD.Libraries;
using System;
using System.Collections.Generic;

namespace AutoCivil
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            List<CADShape> shapes = new List<CADShape>();
            List<CADPlace> places = new List<CADPlace>();

            var quarto = new CADPlace("Quarto");
            places.Add(quarto);

            var quartoShape = new CADShape(quarto)
            {
                Width = 4000,
                Height = 5000
            };
            shapes.Add(quartoShape);

            var banheiroQuarto = new CADPlace("Banheiro Quarto");
            places.Add(banheiroQuarto);
            var banheiroQuartoShape = new CADShape(banheiroQuarto)
            {
                Width = 1000,
                Height = 2000
            };
            shapes.Add(banheiroQuartoShape);

            quartoShape.RightShape = banheiroQuartoShape;

            var lavabo = new CADPlace("Lavabo");
            places.Add(lavabo);
            var lavaboShape = new CADShape(lavabo)
            {
                Width = 800,
                Height = 1500
            };
            shapes.Add(lavaboShape); 

            banheiroQuartoShape.RightShape = lavaboShape;

            var quarto2 = new CADPlace("Quarto 2");
            places.Add(quarto2);
            var quarto2Shape1 = new CADShape(quarto2)
            {
                Width = 3000,
                Height = 1500
            };
            shapes.Add(quarto2Shape1);

            quartoShape.BottomShape = quarto2Shape1;

            var quarto2Shape3 = new CADShape(quarto2)
            {
                Width = 1000,
                Height = 1500
            };
            shapes.Add(quarto2Shape3);
            quarto2Shape1.RightShape = quarto2Shape3;

            var quarto2Shape2 = new CADShape(quarto2)
            {
                Width = 3000,
                Height = 2000
            };
            shapes.Add(quarto2Shape2);
            quarto2Shape1.BottomShape = quarto2Shape2;

            var suite = new CADPlace("Banheiro Quarto 2");
            places.Add(suite);
            var suiteShape = new CADShape(suite)
            {
                Width = 800,
                Height = 1800
            };
            shapes.Add(suiteShape);
            quarto2Shape2.RightShape = suiteShape;
            suiteShape.TopShape = quarto2Shape3;

            var designer = new Designer();
            designer.DesignCAD(@"D:/Niteroi/Projetos/CADs/teste.dxf", shapes, places);

            Console.WriteLine("Design finish !");
        }
    }
}
