# 由 icon_source.png 生成 256x256 的 icon.png，右下角叠加数字 9

Add-Type -AssemblyName System.Drawing

$Size = 256
$Text = "9"
$Source = Join-Path $PSScriptRoot "icon_source.png"
$Output = Join-Path $PSScriptRoot "icon.png"

$src = [System.Drawing.Image]::FromFile($Source)
$bmp = New-Object System.Drawing.Bitmap $Size, $Size
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
$g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
$g.Clear([System.Drawing.Color]::Transparent)
$g.DrawImage($src, 0, 0, $Size, $Size)

$path = New-Object System.Drawing.Drawing2D.GraphicsPath
$family = New-Object System.Drawing.FontFamily "Arial"
$emSize = 150
$format = New-Object System.Drawing.StringFormat
$path.AddString($Text, $family, [int][System.Drawing.FontStyle]::Bold, $emSize, (New-Object System.Drawing.PointF 0, 0), $format)
$bounds = $path.GetBounds()
$matrix = New-Object System.Drawing.Drawing2D.Matrix
$matrix.Translate($Size - $bounds.Right - 14, $Size - $bounds.Bottom - 10)
$path.Transform($matrix)

$outline = New-Object System.Drawing.Pen ([System.Drawing.Color]::FromArgb(255, 10, 20, 40)), 14
$outline.LineJoin = [System.Drawing.Drawing2D.LineJoin]::Round
$g.DrawPath($outline, $path)
$fill = New-Object System.Drawing.Drawing2D.LinearGradientBrush (
    (New-Object System.Drawing.PointF 0, ($Size - $bounds.Height - 10)),
    (New-Object System.Drawing.PointF 0, $Size),
    [System.Drawing.Color]::FromArgb(255, 255, 236, 140),
    [System.Drawing.Color]::FromArgb(255, 255, 160, 30))
$g.FillPath($fill, $path)

$bmp.Save($Output, [System.Drawing.Imaging.ImageFormat]::Png)

$fill.Dispose(); $outline.Dispose(); $matrix.Dispose(); $path.Dispose(); $family.Dispose(); $format.Dispose()
$g.Dispose(); $bmp.Dispose(); $src.Dispose()
Write-Host "Icon created: $Output" -ForegroundColor Green
