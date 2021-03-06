using System.Linq;

namespace CSharpMath.Playground.TypographyTest {
  static class GlyphLayout {
    public static Typography.TextLayout.GlyphLayout Get =>
      new Typography.TextLayout.GlyphLayout(new Typography.OpenFont.OpenFontReader()
          .Read(new System.IO.FileStream(
            System.IO.Directory.EnumerateFiles(Paths.ReferenceFontsFolder, "*.otf").First(),
            System.IO.FileMode.Open)) ?? throw new System.InvalidOperationException("Invalid font!"));
  };
}
