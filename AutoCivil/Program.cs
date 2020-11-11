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

            var cozinha = new CADPlace("Cozinha");
            places.Add(cozinha);

            var cozinhaShape1 = new CADShape(cozinha)
            {
                Width = 6800,
                Height = 3000
            };
            shapes.Add(cozinhaShape1);

            var cozinhaShape2 = new CADShape(cozinha)
            {
                Width = 1700,
                Height = 2500
            };
            shapes.Add(cozinhaShape2);
            cozinhaShape1.RightShape = cozinhaShape2;

            var cozinhaShape3 = new CADShape(cozinha)
            {
                Width = 2450,
                Height = 1500
            };
            shapes.Add(cozinhaShape3);
            cozinhaShape2.RightShape = cozinhaShape3;

            var elevador = new CADPlace("Elevador");
            places.Add(elevador);
            var elevadorShape = new CADShape(elevador)
            {
                Width = 1500,
                Height = 1500
            };
            shapes.Add(elevadorShape);
            cozinhaShape2.BottomShape = elevadorShape;

            var suite = new CADPlace("Suite");
            places.Add(suite);
            var suiteShape = new CADShape(suite)
            {
                Width = 3000,
                Height = 1500
            };
            shapes.Add(suiteShape);
            cozinhaShape3.RightShape = suiteShape;

            var quarto = new CADPlace("Quarto");
            places.Add(quarto);
            var quartoShape = new CADShape(quarto)
            {
                Width = 3000,
                Height = 5900
            };
            shapes.Add(quartoShape);
            suiteShape.RightShape = quartoShape;

            var musculacao = new CADPlace("Musculacao");
            places.Add(musculacao);
            var musculacaoShape = new CADShape(musculacao)
            {
                Width = 3000,
                Height = 3000
            };
            shapes.Add(musculacaoShape);
            suiteShape.BottomShape = musculacaoShape;

            var sala = new CADPlace("Sala");
            places.Add(sala);
            var salaShape1 = new CADShape(sala)
            {
                Width = 6800,
                Height = 1200
            };
            shapes.Add(salaShape1);
            cozinhaShape1.BottomShape = salaShape1;

            var salaShape2 = new CADShape(sala)
            {
                Width = 8500,
                Height = 500
            };
            shapes.Add(salaShape2);
            salaShape1.BottomShape = salaShape2;

            var salaShape3 = new CADShape(sala)
            {
                Width = 8500,
                Height = 9500
            };
            shapes.Add(salaShape3);
            salaShape2.BottomShape = salaShape3;

            var corredor = new CADPlace("Corredor");
            places.Add(corredor);
            var corredorShape1 = new CADShape(corredor)
            {
                Width = 1250,
                Height = 1000
            };
            shapes.Add(corredorShape1);
            salaShape3.RightShape = corredorShape1;

            var corredorShape2 = new CADShape(corredor)
            {
                Width = 4200,
                Height = 1000
            };
            shapes.Add(corredorShape2);
            corredorShape1.RightShape = corredorShape2;

            var banheiro = new CADPlace("Banheiro");
            places.Add(banheiro);
            var banheiroShape1 = new CADShape(banheiro)
            {
                Width = 1000,
                Height = 1500
            };
            shapes.Add(banheiroShape1);
            corredorShape2.TopShape = banheiroShape1;

            var designer = new Designer();
            designer.DesignCAD(@"D:/Niteroi/Projetos/CADs/teste.dxf", shapes, places);

            Console.WriteLine("Design finish !");
        }
    }
}
