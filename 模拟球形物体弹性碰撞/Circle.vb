Public Class Circle
    Public Color As Color '颜色
    Public Radius As Integer '半径
    Public Mass As Integer '质量
    Public Rectangle As Rectangle '矩形
    Public Angle As Integer '运动方向角度
    Public VelocityX As Integer '水平分速度
    Public VelocityY As Integer '垂直分速度
    Private PointInside As Point '坐标

    Public Sub New(SetColor As Color, SetRadius As Integer, SetMass As Integer, SetPointX As Integer, SetPointY As Integer, SetVelocityX As Integer, SetVelocityY As Integer)
        Color = SetColor
        Radius = SetRadius
        'Mass = SetMass'使用球体体积公式自动计算球体质量
        Mass = ((Math.PI * Math.Pow(Radius, 3) * 4) / 3)
        PointInside = New Point(SetPointX, SetPointY)
        Rectangle = New Rectangle(SetPointX - SetRadius / 2, SetPointY - SetRadius / 2, SetRadius, SetRadius)
        VelocityX = SetVelocityX
        VelocityY = SetVelocityY
        Debug.Print("生成 Circle：半径({0})，质量({1})，坐标({2},{3})，速度({4},{5})", Radius, Mass, Point.X, Point.Y, VelocityX, VelocityY)
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
            Rectangle = New Rectangle(value.X - Radius / 2, value.Y - Radius / 2, Radius, Radius)
        End Set
    End Property


End Class
