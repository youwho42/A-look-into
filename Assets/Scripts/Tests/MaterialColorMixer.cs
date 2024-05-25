using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using Unity.Collections;
using UnityEngine;

public static class MaterialColorMixer
{

    public static Color CombineRGBs(Color colorA, Color colorB)
    {
     
        Color a = RGBtoHSV(colorA);
        Color b = RGBtoHSV(colorB);

        
        Color combined = CombineHSVColors(a, b);

        combined = HSVtoRGB(combined.r, combined.g, combined.b);    

       
        
        //Color[] colors = new Color[] {a, b};
        

        //Color combined = CombineCMYK(colors);
        //Debug.Log(combined);
        
        return combined;
    }

    public static Color RGBtoHSV(Color rgb)
    {
        float min = Mathf.Min(rgb.r, Mathf.Min(rgb.g, rgb.b));
        float max = Mathf.Max(rgb.r, Mathf.Max(rgb.g, rgb.b));
        float delta = max - min;

        float hue = 0;
        if (delta != 0)
        {
            if (max == rgb.r)
                hue = Mathf.Repeat((rgb.g - rgb.b) / delta, 6.0f);
            else if (max == rgb.g)
                hue = (rgb.b - rgb.r) / delta + 2.0f;
            else
                hue = (rgb.r - rgb.g) / delta + 4.0f;
        }
        hue *= 60.0f;

        float saturation = max != 0 ? delta / max : 0;

        float value = max;

        return new Color(hue / 360.0f, saturation, value);
    }

    private static Color HSVtoRGB(float h, float s, float v)
    {
        if (s == 0)
            return new Color(v, v, v);

        float c = v * s;
        float x = c * (1 - Mathf.Abs((h / 60) % 2 - 1));
        float m = v - c;

        if (h < 60) return new Color(c + m, x + m, m);
        if (h < 120) return new Color(x + m, c + m, m);
        if (h < 180) return new Color(m, c + m, x + m);
        if (h < 240) return new Color(m, x + m, c + m);
        if (h < 300) return new Color(x + m, m, c + m);
        return new Color(c + m, m, x + m);
    }

    public static Color CombineHSVColors(Color hsvColor1, Color hsvColor2)
    {
        // Take the average of hue, max of saturation, and value
        float hue = (hsvColor1.r + hsvColor2.r) / 2.0f;
        float saturation = Mathf.Max(hsvColor1.g, hsvColor2.g);
        float value = Mathf.Max(hsvColor1.b, hsvColor2.b);

        // Convert HSV to RGB
        return HSVtoRGB(hue, saturation, value);
    }











    //public static Vector3 RGBtoXYZ(Color rgb)
    //{
    //    // Convert sRGB to linear RGB
    //    float rLinear = Mathf.Pow(rgb.r, 2.2f);
    //    float gLinear = Mathf.Pow(rgb.g, 2.2f);
    //    float bLinear = Mathf.Pow(rgb.b, 2.2f);

    //    // RGB to XYZ conversion matrix
    //    float X = rLinear * 0.4124564f + gLinear * 0.3575761f + bLinear * 0.1804375f;
    //    float Y = rLinear * 0.2126729f + gLinear * 0.7151522f + bLinear * 0.0721750f;
    //    float Z = rLinear * 0.0193339f + gLinear * 0.1191920f + bLinear * 0.9503041f;

    //    return new Vector3(X, Y, Z);
    //}
    //public static Color XYZtoRGB(Vector3 xyz)
    //{
    //    // XYZ to RGB conversion matrix (sRGB D65)
    //    Matrix4x4 matrix = new Matrix4x4(
    //        new Vector4(3.2404542f, -1.5371385f, -0.4985314f, 0),
    //        new Vector4(-0.9692660f, 1.8760108f, 0.0415560f, 0),
    //        new Vector4(0.0556434f, -0.2040259f, 1.0572252f, 0),
    //        new Vector4(0, 0, 0, 1)
    //    );

    //    // Convert XYZ to linear RGB
    //    Vector3 linearRGB = matrix * xyz;

    //    // Convert linear RGB to gamma-corrected RGB (sRGB)
    //    float gamma = 2.4f;
    //    float a = 0.055f;
    //    Vector3 gammaCorrectedRGB = new Vector3(
    //        Mathf.Pow(Mathf.Clamp01(linearRGB.x), 1.0f / gamma),
    //        Mathf.Pow(Mathf.Clamp01(linearRGB.y), 1.0f / gamma),
    //        Mathf.Pow(Mathf.Clamp01(linearRGB.z), 1.0f / gamma)
    //    );

    //    // Convert to Unity Color
    //    return new Color(gammaCorrectedRGB.x, gammaCorrectedRGB.y, gammaCorrectedRGB.z);
    //}

    //public static Vector3 XYZtoLAB(Vector3 xyz)
    //{
    //    // Reference white point (D65)
    //    float ref_X = 95.047f;
    //    float ref_Y = 100.000f;
    //    float ref_Z = 108.883f;

    //    // Normalize XYZ values
    //    float x = xyz.x / ref_X;
    //    float y = xyz.y / ref_Y;
    //    float z = xyz.z / ref_Z;

    //    // Convert XYZ to LAB
    //    float epsilon = 0.008856f; // Actual CIE standard
    //    float kappa = 903.3f;       // Actual CIE standard

    //    float f(float t)
    //    {
    //        return t > epsilon ? Mathf.Pow(t, 1.0f / 3.0f) : (kappa * t + 16.0f) / 116.0f;
    //    }

    //    float l = 116.0f * f(y) - 16.0f;
    //    float a = 500.0f * (f(x) - f(y));
    //    float b = 200.0f * (f(y) - f(z));

    //    return new Vector3(l, a, b);
    //}
    //public static Vector3 LABtoXYZ(Vector3 lab)
    //{
    //    // Reference white point (D65)
    //    float ref_X = 95.047f;
    //    float ref_Y = 100.000f;
    //    float ref_Z = 108.883f;

    //    float delta = 6.0f / 29.0f;

    //    float fy = (lab.x + 16.0f) / 116.0f;
    //    float fx = fy + (lab.y / 500.0f);
    //    float fz = fy - (lab.z / 200.0f);

    //    float x = (fx > delta) ? ref_X * fx * fx * fx : (fx - 16.0f / 116.0f) * 3 * delta * delta * ref_X;
    //    float y = (fy > delta) ? ref_Y * fy * fy * fy : (fy - 16.0f / 116.0f) * 3 * delta * delta * ref_Y;
    //    float z = (fz > delta) ? ref_Z * fz * fz * fz : (fz - 16.0f / 116.0f) * 3 * delta * delta * ref_Z;

    //    return new Vector3(x, y, z);
    //}

    //public static Vector3 CombineLABColors(Vector3 labColor1, Vector3 labColor2)
    //{
    //    // Subtract the b* component of the second color from the a* component of the first color
    //    float a = labColor1.y - labColor2.z;
    //    // Subtract the a* component of the second color from the b* component of the first color
    //    float b = labColor1.z - labColor2.y;

    //    // Take the average of L* components
    //    float l = (labColor1.x + labColor2.x) / 2.0f;

    //    return new Vector3(l, a, b);
    //    //// Calculate the average of each component
    //    //float l = (labColor1.x + labColor2.x) / 2.0f;
    //    //float a = (labColor1.y + labColor2.y) / 2.0f;
    //    //float b = (labColor1.z + labColor2.z) / 2.0f;

    //    //return new Vector3(l, a, b);
    //}














    //public static Color RGBtoCMYK(Color rgbColor)
    //{
    //    Debug.Log(rgbColor.r);
    //    float r = rgbColor.r;
    //    float g = rgbColor.g;
    //    float b = rgbColor.b;

    //    //Normalize RGB values
    //    r /= 1f;
    //    g /= 1f;
    //    b /= 1f;

    //    float cyan;
    //    float magenta;
    //    float yellow;
    //    float black;
    //    // Find maximum of RGB
    //    float max = Mathf.Max(r, Mathf.Max(g, b));

    //    // If RGB are all 0, return pure black
    //    if (max == 0)
    //    {
    //        cyan = 0;
    //        magenta = 0;
    //        yellow = 0;
    //        black = 1;
    //        return new Color(cyan, magenta, yellow, black);
    //    }

    //    // Convert RGB to CMY
    //    float c = 1 - (r / max);
    //    float m = 1 - (g / max);
    //    float y = 1 - (b / max);

    //    // Find minimum of CMY
    //    float minCMY = Mathf.Min(c, Mathf.Min(m, y));

    //    // Calculate K (black)
    //    black = minCMY;

    //    // Calculate C, M, Y adjusted for black
    //    cyan = (c - black) / (1 - black);
    //    magenta = (m - black) / (1 - black);
    //    yellow = (y - black) / (1 - black);

    //    return new Color(cyan, magenta, yellow, black);
    //}


    //public static Color CombineCMYK(Color[] cmykColors)
    //{
    //    float maxCyan = 0;
    //    float maxMagenta = 0;
    //    float maxYellow = 0;
    //    float maxBlack = 0;

    //    // Find maximum values for each CMYK component
    //    foreach (Color color in cmykColors)
    //    {
    //        //maxCyan = Mathf.Max(maxCyan, color.r);
    //        //maxMagenta = Mathf.Max(maxMagenta, color.g);
    //        //maxYellow = Mathf.Max(maxYellow, color.b);
    //        //maxBlack = Mathf.Max(maxBlack, color.a);
    //        maxCyan += color.r;
    //        maxMagenta += color.g;
    //        maxYellow += color.b;
    //        maxBlack += color.a;
    //    }

    //    maxCyan /= cmykColors.Length;
    //    maxMagenta /= cmykColors.Length;
    //    maxYellow /= cmykColors.Length;
    //    maxBlack /= cmykColors.Length;

    //    // Return the combined CMYK color
    //    return new Color(maxCyan, maxMagenta, maxYellow, maxBlack);
    //}

    //public static Color CMYKtoRGB(float cyan, float magenta, float yellow, float black)
    //{
    //    float red = (1 - Mathf.Min(1, cyan * (1 - black) + black));
    //    float green = (1 - Mathf.Min(1, magenta * (1 - black) + black));
    //    float blue = (1 - Mathf.Min(1, yellow * (1 - black) + black));

    //    return new Color(red, green, blue);
    //}

}
