namespace CSharpMath.Playground {
  static partial class AlgorithmsTest {
    public static int StringIndexToCodepointIndex(string str, int stringIndex) {
      if (stringIndex < 0)
        throw new System.ArgumentOutOfRangeException(nameof(stringIndex), 
          stringIndex, "The string index is negative.");
      for (int i = 0, count = 0; i < str.Length; i++, count++) {
        if (char.IsSurrogate(str[i])) i++;
        if (i >= stringIndex) return count;
      }
      throw new System.ArgumentOutOfRangeException(nameof(stringIndex), 
        stringIndex, "The string index is beyond the last codepoint of the string.");
    }
    public static void StringIndexToCodepointIndexTest() {
      const string s = "E\uD800\uDC00E\uD800\uDC00\uD800\uDC00ED";
      if (StringIndexToCodepointIndex(s, 0) != 0) throw new System.InvalidOperationException();
      if (StringIndexToCodepointIndex(s, 1) != 1) throw new System.InvalidOperationException();
      if (StringIndexToCodepointIndex(s, 2) != 1) throw new System.InvalidOperationException();
      if (StringIndexToCodepointIndex(s, 3) != 2) throw new System.InvalidOperationException();
      if (StringIndexToCodepointIndex(s, 4) != 3) throw new System.InvalidOperationException();
      if (StringIndexToCodepointIndex(s, 5) != 3) throw new System.InvalidOperationException();
      if (StringIndexToCodepointIndex(s, 6) != 4) throw new System.InvalidOperationException();
      if (StringIndexToCodepointIndex(s, 7) != 4) throw new System.InvalidOperationException();
      if (StringIndexToCodepointIndex(s, 8) != 5) throw new System.InvalidOperationException();
      if (StringIndexToCodepointIndex(s, 9) != 6) throw new System.InvalidOperationException();
    }
  }
}
