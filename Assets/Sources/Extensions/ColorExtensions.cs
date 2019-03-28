using UnityEngine;

public static class ColorExtensions {
    public static Color WithAlpha(this Color inputColor, float alpha) {
        return new Color(inputColor.r, inputColor.g, inputColor.b, alpha);
    }
}
