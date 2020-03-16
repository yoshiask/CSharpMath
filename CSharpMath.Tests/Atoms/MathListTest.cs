using CSharpMath.Atom;
using CSharpMath.Atom.Atoms;
using System;
using System.Linq;
using Xunit;

namespace CSharpMath.Tests.PreTypesetting
{
    using Range = Atom.Range;
    public class MathListTest
    {
        internal static void CheckClone(MathAtom original, MathAtom clone)
        {
            Assert.Equal(original, clone);
            Assert.NotSame(original, clone);
        }
        internal static void CheckClone(MathList original, MathList clone)
        {
            Assert.Equal(original, clone);
            Assert.NotSame(original, clone);
        }

        [Fact]
        public void TestAdd()
        {
            var list = new MathList();
            Assert.Empty(list);
            var atom = MathAtoms.Placeholder;
            list.Add(atom);
            Assert.Single(list);
            Assert.Equal(atom, list[0]);
            var atom2 = MathAtoms.Times;
            list.Add(atom2);
            Assert.Equal(2, list.Count);
            Assert.Equal(atom2, list[1]);
        }

        [Fact]
        public void TestAddErrors()
        {
            var list = new MathList();
            Assert.Throws<ArgumentNullException>(() => list.Add(null!));
        }

        [Fact]
        public void TestInsert()
        {
            var list = new MathList();
            Assert.Empty(list);
            var atom = MathAtoms.Placeholder;
            list.Insert(0, atom);
            Assert.Single(list);
            Assert.Equal(atom, list[0]);
            var atom2 = new LargeOperator("+", false);
            list.Insert(0, atom2);
            Assert.Equal(2, list.Count);
            Assert.Equal(atom2, list[0]);
            Assert.Equal(atom, list[1]);
            var atom3 = new Variable("x");
            list.Insert(2, atom3);
            Assert.Equal(3, list.Count);
            Assert.Equal(atom2, list[0]);
            Assert.Equal(atom, list[1]);
            Assert.Equal(atom3, list[2]);
        }

        [Fact]
        public void TestAppend()
        {
            var list1 = new MathList();
            var atom1 = MathAtoms.Placeholder;
            var atom2 = new LargeOperator("+", false);
            var atom3 = new LargeOperator("-", false);
            list1.Add(atom1);
            list1.Add(atom2);
            list1.Add(atom3);

            var list2 = new MathList();
            var atom5 = MathAtoms.Times;
            var atom6 = MathAtoms.Divide;
            list2.Add(atom5);
            list2.Add(atom6);

            Assert.Equal(3, list1.Count);
            Assert.Equal(2, list2.Count);

            list1.Append(list2);
            Assert.Equal(5, list1.Count);
            Assert.Equal(atom5, list1[3]);
            Assert.Equal(atom6, list1[4]);
        }

        [Fact]
        public void TestRemoveAtomAtIndex()
        {
            var list = new MathList();
            var atom = MathAtoms.Placeholder;
            var atom2 = MathAtoms.Times;
            list.Add(atom);
            list.Add(atom2);
            Assert.Equal(2, list.Count);
            list.RemoveAt(0);
            Assert.Single(list);
            Assert.Equal(atom2, list[0]);
            Assert.Throws<ArgumentOutOfRangeException>(() => list.RemoveAt(1));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.RemoveAt(2));
        }

        [Fact]
        public void TestRemoveAtomsInRange()
        {
            var list = new MathList();
            var atom = MathAtoms.Placeholder;
            var atom2 = MathAtoms.Times;
            var atom3 = MathAtoms.Divide;
            list.Add(atom);
            list.Add(atom2);
            list.Add(atom3);
            Assert.Equal(3, list.Count);
            list.RemoveAtoms(new Atom.Range(1, 2));
            Assert.Single(list);
            Assert.Equal(atom, list[0]);
            Assert.Throws<ArgumentException>(() => list.RemoveAtoms(new Atom.Range(1, 1)));
        }

        [Fact]
        public void TestListCopy()
        {
            var list = new MathList();
            var atom = MathAtoms.Placeholder;
            var atom2 = MathAtoms.Times;
            var atom3 = MathAtoms.Divide;
            list.Add(atom);
            list.Add(atom2);
            list.Add(atom3);

            var list2 = list.Clone(false);
            CheckClone(list, list2);
        }

        [Fact]
        public void TestListCopyWithFusedItems()
        {
            var builder = new LaTeXBuilder("12+x");
            var list = builder.Build();

            var finalized = list.FinalizedList();
            var fusedCount = finalized.Sum(atom => atom.FusedAtoms?.Count ?? 0);
            Assert.Equal(2, fusedCount);

            var copy = finalized.Clone(true);
            var fusedCopyCount = copy.Sum(atom => atom.FusedAtoms?.Count ?? 0);
            Assert.Equal(2, fusedCopyCount);
        }

        [Fact]
        public void TestListFinalizedCopy()
        {
            var input = @"-52x^{13+y}_{15-} + (-12.3 *)\frac{-12}{15.2}";
            var list = LaTeXBuilder.MathListFromLaTeX(input);
            var finalized = list.Clone(true);
            CheckListContents(finalized);
            var reFinalized = finalized.Clone(true);
            CheckListContents(reFinalized);

            static void CheckListContents(MathList list)
            {
                Assert.Equal(10, list.Atoms.Count);
                var atom0 = list.Atoms[0];
                Assert.IsType<UnaryOperator>(atom0);
                Assert.Equal("\u2212", atom0.Nucleus);
                Assert.Equal(new Range(0, 1), atom0.IndexRange);
                var atom1 = list.Atoms[1];
                Assert.IsType<Number>(atom1);
                Assert.Equal("52", atom1.Nucleus);
                Assert.Equal(new Range(1, 2), atom1.IndexRange);
                var atom2 = list.Atoms[2];
                Assert.IsType<Variable>(atom2);
                Assert.Equal("x", atom2.Nucleus);
                Assert.Equal(new Range(3, 1), atom2.IndexRange);
                var superScript = atom2.Superscript;
                Assert.Equal(3, superScript.Atoms.Count);

                var super0 = superScript.Atoms[0];
                Assert.IsType<Number>(super0);
                Assert.Equal("13", super0.Nucleus);
                Assert.Equal(new Range(0, 2), super0.IndexRange);
                var super1 = superScript.Atoms[1];
                Assert.IsType<BinaryOperator>(super1);
                Assert.Equal("+", super1.Nucleus);
                Assert.Equal(new Range(2, 1), super1.IndexRange);
                var super2 = superScript.Atoms[2];
                Assert.IsType<Variable>(super2);
                Assert.Equal("y", super2.Nucleus);
                Assert.Equal(new Range(3, 1), super2.IndexRange);
            }
        }
    }
}
