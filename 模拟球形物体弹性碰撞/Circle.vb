Imports System.Drawing.Drawing2D

Public Class Circle
    Public Color As Color '颜色
    Public Radius As Integer '半径
    Public Mass As Integer '质量
    Public LastAngle As Integer '上一次速度的角度，用于判断球体图像顺时针还是逆时针旋转
    Public Rectangle As Rectangle '矩形
    Public VelocityX As Integer '水平分速度
    Public VelocityY As Integer '垂直分速度
    Public Image As Bitmap '要绘制的图像
    Public ClockwiseRotate As Boolean '角度 Angle 顺时针旋转
    Private PointInside As Point '坐标
    Private AngleInside As Integer '图像旋转的角度

    Public Sub New(SetColor As Color, SetRadius As Integer, SetMass As Integer, SetPointX As Integer, SetPointY As Integer, SetVelocityX As Integer, SetVelocityY As Integer)
        Color = SetColor
        Radius = SetRadius
        'Mass = SetMass'使用球体体积公式自动计算球体质量
        Mass = ((Math.PI * Math.Pow(Radius, 3) * 4) / 3)
        PointInside = New Point(SetPointX, SetPointY)
        Rectangle = New Rectangle(SetPointX - SetRadius, SetPointY - SetRadius, SetRadius * 2, SetRadius * 2)
        VelocityX = SetVelocityX
        VelocityY = SetVelocityY
        LastAngle = -Math.Atan2(VelocityX, VelocityY) * 180 / Math.PI
        'Debug.Print("生成 Circle：半径({0})，质量({1})，坐标({2},{3})，速度({4},{5})", Radius, Mass, Point.X, Point.Y, VelocityX, VelocityY)
    End Sub

    ''' <summary>
    ''' 定义为 Point 属性，方便外部设置 Point 时顺带修改 Rectangle
    ''' </summary>
    ''' <returns>返回当前圆心坐标</returns>
    Public Property Point As Point
        Get
            Return PointInside
        End Get
        Set(value As Point)
            PointInside = value
            Rectangle = New Rectangle(value.X - Radius, value.Y - Radius, Radius * 2, Radius * 2)
        End Set
    End Property

    Public Property Angle As Integer
        Get
            Return AngleInside
        End Get
        Set(value As Integer)
            AngleInside = value
            Image = GetRotateBitmap(My.Resources.UnityResource.face, AngleInside)
        End Set
    End Property

    ''' <summary>
    ''' 返回旋转任意角度后的图像
    ''' </summary>
    ''' <param name="BitmapRes">需要旋转处理的图像</param>
    ''' <param name="Angle">旋转角度[单位：度]</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetRotateBitmap(ByVal BitmapRes As Bitmap, ByVal Angle As Single) As Bitmap
        Dim ReturnBitmap As New Bitmap(BitmapRes.Width, BitmapRes.Height)
        Dim MyGraphics As Graphics = Graphics.FromImage(ReturnBitmap)
        MyGraphics.SmoothingMode = SmoothingMode.HighQuality
        MyGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality

        MyGraphics.TranslateTransform(BitmapRes.Width / 2, BitmapRes.Height / 2)
        MyGraphics.RotateTransform(Angle, MatrixOrder.Prepend)

        MyGraphics.TranslateTransform(-BitmapRes.Width / 2, -BitmapRes.Height / 2)
        MyGraphics.DrawImage(BitmapRes, 0, 0, BitmapRes.Width, BitmapRes.Height)
        MyGraphics.Dispose()
        GC.Collect()
        Return ReturnBitmap
    End Function
End Class
