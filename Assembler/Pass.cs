namespace Caylang.Assembler
{
	public class Pass
	{
		public virtual void Visit(ParseTreeBranch node)
		{
			foreach (var child in node.Children)
			{
				switch (child)
				{
					case ParseTreeBranch branch:
						Visit(branch);
						break;
					case ParseTreeLeaf leaf:
						Visit(leaf);
						break;
				};
			}
		}

		public virtual void Visit(ParseTreeLeaf node) { }
	}
}
