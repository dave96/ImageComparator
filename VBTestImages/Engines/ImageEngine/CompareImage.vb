Imports System.IO
Imports System.Drawing

Namespace ImageEngine

    Public Class CompareImage
        Dim FirstImage, SecondImage As Bitmap

        Public Sub New(ByRef FirstImage As Stream, ByRef SecondImage As Stream)
            Me.FirstImage = Image.FromStream(FirstImage)
            Me.SecondImage = Image.FromStream(SecondImage)

            If Me.FirstImage.Width <> Me.SecondImage.Width OrElse Me.FirstImage.Height <> Me.SecondImage.Height Then
                Throw New ArgumentException("Dimensions of the images are not equal.")
            End If

        End Sub

        Public Function SequentialCompare(fuzz As UInt32, horizontalPlay As UInt32, verticalPlay As UInt32, Background As Color, Highlight As Color) As Stream
            If fuzz < 0 OrElse fuzz > 100 Then Throw New ArgumentException("Fuzz has to be between 0 and 100")

            Dim Result As New Bitmap(FirstImage.Width, FirstImage.Height)
            SequentialPaint(Result, Background)

            For i = 0 To Result.Width - 1
                For j = 0 To Result.Height - 1
                    If CheckPixel(i, j, fuzz, verticalPlay, horizontalPlay) Then Result.SetPixel(i, j, Highlight)
                Next
            Next

            Dim res As New MemoryStream()
            Result.Save(res, Imaging.ImageFormat.Jpeg)

            Return res
        End Function

        Private Sub SequentialPaint(ByRef BitmapToPaint As Bitmap, ColorToFill As Color)
            For i = 0 To BitmapToPaint.Width - 1
                For j = 0 To BitmapToPaint.Height - 1
                    BitmapToPaint.SetPixel(i, j, ColorToFill)
                Next
            Next
        End Sub

        Private Function CheckPixel(x As Int32, y As Int32, fuzz As UInt32, horizontalPlay As UInt32, verticalPlay As UInt32) As Boolean
            For i = Math.Max(0, x - horizontalPlay) To Math.Min(FirstImage.Width - 1, x + horizontalPlay)
                For j = Math.Max(0, y - verticalPlay) To Math.Min(FirstImage.Height - 1, y + verticalPlay)
                    If CompareColors(FirstImage.GetPixel(x, y), SecondImage.GetPixel(i, j)) < fuzz Then Return False
                Next
            Next
            Return True
        End Function

        Private Function CompareColors(f As Color, s As Color) As Double
            Dim r, g, b, a As Double
            r = (f.R / Byte.MaxValue - s.R / Byte.MaxValue) * (f.R / Byte.MaxValue - s.R / Byte.MaxValue)
            g = (f.G / Byte.MaxValue - s.G / Byte.MaxValue) * (f.G / Byte.MaxValue - s.G / Byte.MaxValue)
            b = (f.B / Byte.MaxValue - s.B / Byte.MaxValue) * (f.B / Byte.MaxValue - s.B / Byte.MaxValue)
            a = ((f.A / Byte.MaxValue) * (s.A / Byte.MaxValue)) / 3.0
            Return 100 * Math.Sqrt((r + g + b) * a + ((f.A / Byte.MaxValue - s.A / Byte.MaxValue) * (f.A / Byte.MaxValue - s.A / Byte.MaxValue)))
        End Function

    End Class

End Namespace