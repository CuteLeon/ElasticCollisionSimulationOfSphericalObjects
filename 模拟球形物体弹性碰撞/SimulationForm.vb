Public Class SimulationForm
    Private Declare Function ReleaseCapture Lib "user32" () As Integer
    Private Declare Function SendMessageA Lib "user32" (ByVal hwnd As Integer, ByVal wMsg As Integer, ByVal wParam As Integer, lParam As VariantType) As Integer

    'TODO: 判断旋转方向是顺时针还是逆时针的算法有逻辑错误（，但是现在的效果也不错，懒得修改了）
    Dim UnityRectangle As Rectangle = New Rectangle(0, 0, 500, 360)
    Dim UnityBitmap As Bitmap
    Dim UnityGraphics As Graphics
    Dim CircleList As New ArrayList

    Private Sub SimulationForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.UnityResource.模拟球形物体弹性碰撞
        'Debug.Print("——————————————————————")
        'Debug.Print("# 程序启动！ {0} #", Now.ToString)
        Dim NewCircle As Circle
        For index As Integer = 0 To 9
            NewCircle = New Circle(
             Rnd() * 10 + 10, Rnd() * UnityRectangle.Width, Rnd() * UnityRectangle.Height, Rnd() * 20 - 10, Rnd() * 20 - 10)
            CircleList.Add(NewCircle)
        Next
        'Debug.Print("# 生成 Circle 对象完毕！ {0} #", Now.ToString)
        'Debug.Print("——————————————————————")

        DrawCircleList()
    End Sub

    Private Sub UnityTimer_Tick(sender As Object, e As EventArgs) Handles UnityTimer.Tick
        DrawCircleList()
    End Sub

    ''' <summary>
    ''' 把 CircleList 里的每一个元素绘制到 UnityBitmap，并显示到窗体背景
    ''' </summary>
    Private Sub DrawCircleList()
        Dim CircleInstance As Circle
        UnityBitmap = My.Resources.UnityResource.UnityBackGround
        UnityGraphics = Graphics.FromImage(UnityBitmap)
        Randomize()

        For Index As Integer = 0 To CircleList.Count - 1
            CircleInstance = GetCircle(CircleList(Index))
            UpdateCircle(CircleInstance, Index)

            With CircleInstance
                'UnityGraphics.FillEllipse(New SolidBrush(.Color), .Rectangle)
                UnityGraphics.DrawImage(.Image, .Rectangle)
                'UnityGraphics.DrawLine(Pens.Red, .Point, New Point(.Point.X + .VelocityX, .Point.Y + .VelocityY))
                'UnityGraphics.DrawString(Index.ToString, Me.Font, Brushes.Yellow, .Point)
            End With
        Next

        Me.BackgroundImage = UnityBitmap
        'Debug.Print("# 刷新了一次界面！ {0} #", Now.ToString)
        GC.Collect()
    End Sub

    ''' <summary>
    ''' 把 CircleList 元素转换为 Circle 类型
    ''' </summary>
    ''' <param name="CircleObject"></param>
    ''' <returns></returns>
    Private Function GetCircle(ByVal CircleObject As Object) As Circle
        Return CType(CircleObject, Circle)
    End Function

    ''' <summary>
    ''' 计算弹性碰撞之后的物体速度（需要分别计算 X 和 Y 轴速度）
    ''' </summary>
    ''' <param name="MassA">物体 A 的质量</param>
    ''' <param name="MassB">物体 B 的质量</param>
    ''' <param name="VelocityA">物体 A 的速度</param>
    ''' <param name="VelocityB">物体 B 的速度</param>
    ''' <param name="VelocityAAfter">物体 A 碰撞后的速度（需要传入地址）</param>
    ''' <param name="VelocityBAfter">物体 B 碰撞后的速度（需要传入地址）</param>
    Private Sub GetVelocityAfterCollision(MassA As Integer, MassB As Integer, VelocityA As Integer, VelocityB As Integer, ByRef VelocityAAfter As Integer, ByRef VelocityBAfter As Integer)
        VelocityAAfter = (VelocityA * (MassA - MassB) + 2 * MassB * VelocityB) / (MassA + MassB)
        VelocityBAfter = (VelocityB * (MassB - MassA) + 2 * MassA * VelocityA) / (MassA + MassB)
    End Sub

    ''' <summary>
    ''' 更新 Circle 对象的状态
    ''' </summary>
    ''' <param name="CircleObject">传入的 Circle 对象地址</param>
    ''' <param name="Index">Circle 对象在 CircleList 中的数字标识，用于向后遍历的前节点，防止重复碰撞</param>
    Private Sub UpdateCircle(ByRef CircleObject As Object, Index As Integer)
        Dim CircleInstance As Circle = GetCircle(CircleList(Index))
        Dim CircleCollideObject As Circle
        Dim NewAngle As Integer
        With CircleInstance
            '更新坐标
            .Point = New Point(.Point.X + .VelocityX, .Point.Y + .VelocityY)
            '更新角度
            .Angle = (.Angle + IIf(.ClockwiseRotate, 10, -10) + 360) Mod 360
            '与墙壁弹性碰撞（使用 Math.Abs 可以防止与墙壁粘连；分开比较水平与垂直方向防止球体逃逸）
            If .Rectangle.Left <= 0 Then
                .Point = New Point(.Radius, .Point.Y)
                .VelocityX = Math.Abs(.VelocityX)
                'Debug.Print("{0} 碰撞到了 左边 墙壁！{1}", Index, Now.ToString)
            ElseIf .Rectangle.Right >= UnityRectangle.Width Then
                .Point = New Point(UnityRectangle.Width - .Radius, .Point.Y)
                .VelocityX = -Math.Abs(.VelocityX)
                'Debug.Print("{0} 碰撞到了 右边 墙壁！{1}", Index, Now.ToString)
            End If
            If .Rectangle.Top <= 0 Then
                .Point = New Point(.Point.X, .Radius)
                .VelocityY = Math.Abs(.VelocityY)
                'Debug.Print("{0} 碰撞到了 上边 墙壁！{1}", Index, Now.ToString)
            ElseIf .Rectangle.Bottom >= UnityRectangle.Height Then
                .Point = New Point(.Point.X, UnityRectangle.Height - .Radius)
                .VelocityY = -Math.Abs(.VelocityY)
                'Debug.Print("{0} 碰撞到了 下边 墙壁！{1}", Index, Now.ToString)
            End If
            '与球形弹性碰撞
            For IndexCollide As Integer = Index + 1 To CircleList.Count - 1
                CircleCollideObject = GetCircle(CircleList(IndexCollide))
                If IsCirclesInCollision(CircleCollideObject, CircleInstance) Then
                    GetVelocityAfterCollision(.Mass, CircleCollideObject.Mass, .VelocityX, CircleCollideObject.VelocityX, .VelocityX, CircleCollideObject.VelocityX)
                    GetVelocityAfterCollision(.Mass, CircleCollideObject.Mass, .VelocityY, CircleCollideObject.VelocityY, .VelocityY, CircleCollideObject.VelocityY)
                    '碰撞之后立即移动，可以减少发生粘连的概率
                    CircleCollideObject.Point = New Point(CircleCollideObject.Point.X + CircleCollideObject.VelocityX, CircleCollideObject.Point.Y + CircleCollideObject.VelocityY)
                    .Point = New Point(.Point.X + .VelocityX, .Point.Y + .VelocityY)
                    '计算旋转角度和旋转方向
                    NewAngle = -Math.Atan2(.VelocityX, .VelocityY) * 180 / Math.PI
                    .ClockwiseRotate = (NewAngle > .LastAngle)
                    .LastAngle = NewAngle
                    'Debug.Print("{0} 与 {1} 发生碰撞！{2}", Index, IndexCollide, Now.ToString)
                End If
            Next
        End With
    End Sub

    ''' <summary>
    ''' 判断两个 Circle 对象是否相交或相切
    ''' </summary>
    ''' <param name="CircleA"></param>
    ''' <param name="CircleB"></param>
    ''' <returns></returns>
    Private Function IsCirclesInCollision(ByVal CircleA As Circle, ByVal CircleB As Circle) As Boolean
        Dim Distance As Integer
        Distance = Math.Pow(CircleA.Point.X - CircleB.Point.X, 2) + Math.Pow(CircleA.Point.Y - CircleB.Point.Y, 2)
        Return (Distance <= Math.Pow(CircleA.Radius + CircleB.Radius, 2))
    End Function

    Private Sub SimulationForm_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
        ReleaseCapture()
        SendMessageA(Me.Handle, &HA1, 2, 0&)
    End Sub

End Class
