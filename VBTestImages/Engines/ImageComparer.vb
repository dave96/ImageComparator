Imports System.IO
Imports System.Threading
Imports System.Threading.Tasks

Public Class ImageComparer
    Public Property FirstImage As String
    Public Property SecondImage As String
    Private LastResult As String

    Private Const Fuzz As UInt32 = 75
    Private Const ResultPath As String = "A:\Archivos\Descargas\test\muynais.jpg"
    Private Const ShiftPath As String = "A:\Archivos\Descargas\test\tmp-shift<>.jpg"
    Private Const CompPath As String = "A:\Archivos\Descargas\test\tmp-comp<>.jpg"

    Public Sub New(FirstImage As String, SecondImage As String)
        Me.FirstImage = FirstImage
        Me.SecondImage = SecondImage
    End Sub

    Public Sub New()
        Me.New(Nothing, Nothing)
    End Sub

    Public Sub ExecuteComparison()
        Dim sync As New Object()

        FileIO.FileSystem.CopyFile(FirstImage, ResultPath, True)

        Parallel.For(-2, 2, Sub(i)
                                If IsNothing(Thread.CurrentThread.ManagedThreadId) Then Throw New Exception()
                                Dim shiftLocal As String = ShiftPath.Replace("<>", Thread.CurrentThread.ManagedThreadId.ToString())
                                Dim compLocal As String = CompPath.Replace("<>", Thread.CurrentThread.ManagedThreadId.ToString())

                                Dim Wrapper As New Imagick.Wrapper()
                                For j = -2 To 2
                                    Wrapper.Shift(SecondImage, shiftLocal, i, j, "white")
                                    Dim res As String = Wrapper.Compare(FirstImage, shiftLocal, compLocal, True, Imagick.CompareMetric.AbsoluteError, Fuzz, "-highlight-color black")
                                    Dim tmpRes As UInt32

                                    While Not UInt32.TryParse(res, tmpRes)
                                        res = Wrapper.Compare(FirstImage, shiftLocal, compLocal, True, Imagick.CompareMetric.AbsoluteError, Fuzz, "-highlight-color black")
                                    End While

                                    SyncLock sync
                                        Wrapper.Mask(ResultPath, compLocal, ResultPath)
                                    End SyncLock

                                Next

                                FileIO.FileSystem.DeleteFile(compLocal)
                                FileIO.FileSystem.DeleteFile(shiftLocal)
                            End Sub)
    End Sub

    Private Function IsFileReady(sFilename As String) As Boolean
        Try
            Using inputStream As FileStream = File.Open(sFilename, FileMode.Open, FileAccess.ReadWrite, FileShare.None)
                If (inputStream.Length > 0) Then
                    Return True
                Else
                    Return False
                End If
            End Using
        Catch ex As Exception
            Return False
        End Try
    End Function
End Class
