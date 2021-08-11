namespace Caylang.Assembler
{
	public class Pass
	{
		public virtual void Visit(ParseTree node)
		{
			switch (node)
			{
				case ParseTreeBranch branch:
					Visit(branch);
					break;
				case ParseTreeLeaf leaf:
					Visit(leaf);
					break;
			};
		}

		public virtual void Visit(ParseTreeBranch node)
		{
			foreach (var child in node.Children)
			{
				Visit(child);
			}
		}

		public virtual void Visit(ParseTreeLeaf node) { }
	}
}
