Namespace Imagick

    Public Class Wrapper
        Private ImagickRoute As String

        Public Sub New()
            ImagickRoute = "A:\Archivos\Descargas\ImageMagick-7.0.4-3-portable-Q16-x64\"
        End Sub

        Public Sub New(ImagickRoute As String)
            Me.ImagickRoute = ImagickRoute
        End Sub

        Public Function Compare(FirstImage As String, SecondImage As String, ResultPath As String, Compose As Boolean, Metric As CompareMetric, FuzzValue As UInt32, Optional AdditionalArgs As String = Nothing) As String
            Dim ImRef As New Process()

            If String.IsNullOrEmpty(FirstImage) Then Throw New ArgumentNullException("First image path can't be null.")
            If String.IsNullOrEmpty(SecondImage) Then Throw New ArgumentNullException("Second image path can't be null.")
            If String.IsNullOrEmpty(ResultPath) Then Throw New ArgumentNullException("Result image path can't be null.")

            ImRef.StartInfo.FileName = String.Concat(ImagickRoute, "compare.exe")
            ImRef.StartInfo.RedirectStandardOutput = True
            ImRef.StartInfo.RedirectStandardError = True
            ImRef.StartInfo.UseShellExecute = False
            ImRef.StartInfo.Arguments = ""

            If Metric <> CompareMetric.None Then
                ImRef.StartInfo.Arguments &= "-metric " & TranslateMetric(Metric)
            End If

            If FuzzValue <> 0 Then
                ImRef.StartInfo.Arguments &= " -fuzz " & FuzzValue.ToString() & "%"
            End If

            If Compose Then
                ImRef.StartInfo.Arguments &= " -compose Src"
            End If

            If Not String.IsNullOrEmpty(AdditionalArgs) Then
                ImRef.StartInfo.Arguments &= " " & AdditionalArgs
            End If

            ImRef.StartInfo.Arguments &= " " & FirstImage & " " & SecondImage & " " & ResultPath

            ImRef.Start()
            ImRef.WaitForExit()

            Dim final As String = ImRef.StandardOutput.ReadToEnd()

            If String.IsNullOrEmpty(final) Then final = ImRef.StandardError.ReadToEnd()

            Return final
        End Function

        Public Sub Shift(Image As String, NewImage As String, x As Int32, y As Int32, Optional Background As String = "none")
            Dim ImRef As New Process()

            If String.IsNullOrEmpty(Image) Then Throw New ArgumentNullException("Image path can't be null.")
            If String.IsNullOrEmpty(NewImage) Then Throw New ArgumentNullException("New Image path can't be null.")

            ImRef.StartInfo.FileName = String.Concat(ImagickRoute, "convert.exe")
            ImRef.StartInfo.RedirectStandardOutput = True
            ImRef.StartInfo.UseShellExecute = False
            ImRef.StartInfo.Arguments = "-page " & ConvertCoords(x, y) & " " & Image & " -background " & Background & " -flatten " & NewImage

            ImRef.Start()
            ImRef.WaitForExit()
        End Sub

        Public Sub Mask(Image As String, Mask As String, NewImage As String)
            Dim ImRef As New Process()

            If String.IsNullOrEmpty(Image) Then Throw New ArgumentNullException("Image path can't be null.")
            If String.IsNullOrEmpty(NewImage) Then Throw New ArgumentNullException("New Image path can't be null.")
            If String.IsNullOrEmpty(Mask) Then Throw New ArgumentNullException("Mask path can't be null.")

            ImRef.StartInfo.FileName = String.Concat(ImagickRoute, "convert.exe")
            ImRef.StartInfo.RedirectStandardOutput = True
            ImRef.StartInfo.UseShellExecute = False
            ImRef.StartInfo.Arguments = Mask & " " & Image & " -compose Screen -composite " & NewImage

            ImRef.Start()
            ImRef.WaitForExit()
        End Sub

        Private Function ConvertCoords(x As Int32, y As Int32) As String
            Dim res As String = ""
            If x >= 0 Then res &= "+"
            res &= x.ToString()
            If y >= 0 Then res &= "+"
            res &= y.ToString()
            Return res
        End Function

        Private Function TranslateMetric(Metric As CompareMetric) As String
            Select Case Metric
                Case CompareMetric.AbsoluteError
                    Return "AE"
                Case CompareMetric.MeanColorDistance
                    Return "FUZZ"
                Case CompareMetric.MeanAbsoluteError
                    Return "MAE"
                Case CompareMetric.MeanErrorPerPixel
                    Return "MEPP"
                Case CompareMetric.MeanErrorSquared
                    Return "MSE"
                Case CompareMetric.NormalizedCrossCorrelation
                    Return "NCC"
                Case CompareMetric.PeakAbsolute
                    Return "PAE"
                Case CompareMetric.PerceptualHash
                    Return "PHASH"
                Case CompareMetric.PeakSignalToNoiseRatio
                    Return "PSNR"
                Case CompareMetric.RootMeanSquared
                    Return "RMSE"
                Case Else
                    Throw New Exception("Invalid Argument in TranslateMetric (bug)")
            End Select
        End Function

    End Class

    Public Enum CompareMetric
        AbsoluteError ' AE
        MeanColorDistance ' FUZZ
        MeanAbsoluteError ' MAE
        MeanErrorPerPixel ' MEPP
        MeanErrorSquared ' MSE
        NormalizedCrossCorrelation ' NCC
        PeakAbsolute ' PAE
        PerceptualHash ' PHASH
        PeakSignalToNoiseRatio ' PSNR
        RootMeanSquared ' RMSE
        None
    End Enum


End Namespace