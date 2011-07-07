using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    public class CbinaryTree
    {
        int lchild;
        int rchild;
        int parent;
        int value;
        public CbinaryTree(int in_lchild, int in_rchild, int in_parent, int in_value)
        {
            lchild = in_lchild;
            rchild = in_rchild;
            parent = in_parent;
            value = in_value;
        }
    }
}
