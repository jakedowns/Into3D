fixed3 RGBToXYZ(fixed3 color) {
    fixed3 xyz = fixed3(0, 0, 0);
    xyz.x = dot(color, fixed3(0.4124, 0.3576, 0.1805));
    xyz.y = dot(color, fixed3(0.2126, 0.7152, 0.0722));
    xyz.z = dot(color, fixed3(0.0193, 0.1192, 0.9505));
    return xyz;
}

fixed3 XYZToRGB(fixed3 color) {
    fixed3 rgb = fixed3(0, 0, 0);
    rgb.x = dot(color, fixed3(3.2406, -1.5372, -0.4986));
    rgb.y = dot(color, fixed3(-0.9689, 1.8758, 0.0415));
    rgb.z = dot(color, fixed3(0.0557, -0.2040, 1.0570));
    return rgb;
}

fixed3 RGBToLAB(fixed3 color) {
    fixed3 lab = fixed3(0, 0, 0);
    fixed3 xyz = RGBToXYZ(color);
    lab.x = 116 * xyz.y - 16;
    lab.y = 500 * (xyz.x - xyz.y);
    lab.z = 200 * (xyz.y - xyz.z);
    return lab;
}

fixed3 LABToRGB(fixed3 lab) {
    fixed3 xyz = fixed3(0, 0, 0);
    xyz.x = lab.y / 500 + (lab.x + 16) / 116;
    xyz.y = lab.x / 116;
    xyz.z = -lab.z / 200 + xyz.y;
    return XYZToRGB(xyz);
}
