Imports System.Drawing
Imports System.IO
Module MainModule

    Sub Main()
        Dim FirstImage As New FileStream("A:\Archivos\Descargas\test\original.jpg", FileMode.Open)
        Dim SecondImage As New FileStream("A:\Archivos\Descargas\test\foto-mal.jpg", FileMode.Open)
        Dim engine As New ImageEngine.CompareImage(FirstImage, SecondImage)

        Console.Write("Fuzz factor (0-100): ")

        Dim fuzz As UInt32 = UInt32.Parse(Console.ReadLine())

        Console.Write("Play: ")

        Dim play As UInt32 = UInt32.Parse(Console.ReadLine())

        Console.WriteLine("Empezamos comparación...")

        Dim newImage As Stream = engine.SequentialCompare(fuzz, play, play, Drawing.Color.White, Drawing.Color.Black)

        Dim Result As Image = Image.FromStream(newImage)
        Result.Save("A:\Archivos\Descargas\test\resultado.jpg")

        FirstImage.Close()
        SecondImage.Close()

        Console.WriteLine("Conversión realizada")

        While True
        End While
    End Sub

End Module
