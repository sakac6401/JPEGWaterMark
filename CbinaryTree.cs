using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    public class CbinaryTree
    {
        public int sum_nodes;
        public int sum_reaves;
        public Node[] nodes;
        public byte[] depth_reaf_num;
        public int[][] depth_table = new int[16][];
        public int[] t_reaf;
        int max_length = 0;

        public CbinaryTree(byte[] in_data)
        {
            try
            {
                sum_nodes = CountNodes(in_data);
                nodes = new Node[sum_nodes];
                depth_reaf_num = (byte[])in_data.Clone();
                for (int i = 0; i < sum_nodes; i++)
                {
                    nodes[i] = new Node();
                }

                MakeDepthTable(in_data);
                CountReaves(in_data);
                SetNodes(in_data);
                SetTReaf();
            }
            catch
            {
                Console.WriteLine("");
            }
        }

        void MakeDepthTable(byte[] in_data)
        {
            int depth_now=0;
            int num_depth = 2;

            for (int i = 1; i < sum_nodes;)
            {
                depth_table[depth_now] = new int[num_depth];
                for (int j = 0; j < num_depth; j++)
                {
                    depth_table[depth_now][j] = i;
                    i++;
                }
                num_depth = (num_depth - in_data[depth_now]) * 2;
                depth_now++;
            }
        }
        void CountReaves(byte[] in_data)
        {
            sum_reaves = 0;
            for (int i = 0; i < in_data.Length; i++)
            {
                sum_reaves += in_data[i];
            }
        }

        int CountNodes(byte[] in_data)
        {
            for (int i = 0; i < 16; i++)
            {
                if (in_data[i] != 0)
                {
                    max_length = i;
                }
            }

            int num_node = 1;
            int num_node_pre = 1;

            for (int i = 0; i < max_length; i++)
            {
                num_node += (num_node_pre * 2);
                num_node_pre *= 2;
                num_node_pre -= in_data[i];                
            }
            num_node += (num_node_pre * 2);

            return num_node;
        }
        void SetNodes(byte[] in_data)
        {
            int num_depth=2;
            int now_depth=1;
            //rootの設定
            nodes[0].depth = 0;

            //深さ順に整列
            for (int i = 1; i < sum_nodes;)
            {
                for (int j = 0; j < num_depth; j++)
                {
                    nodes[i].depth = now_depth;
                    if (depth_reaf_num[now_depth - 1] > j)
                    {
                        nodes[i].is_reaf = true;
                    }
                    i++;
                }
                num_depth = (num_depth - in_data[now_depth-1]) * 2;
                now_depth++;
            }
            nodes[sum_nodes-1].is_reaf = true;

            //親子関係整列
            int count = 1;
            for (int i = 0; i < sum_nodes; i++)
            {
                if (nodes[i].is_reaf == false)
                {
                    for (int j = 0; j < 2; j++, count++)
                    {
                        nodes[i].child[j] = count;
                        nodes[nodes[i].child[j]].parent = i;
                    }
                }
            }

        }
        public void SetValue(byte[] in_data)
        {
            int count = 0;
            try
            {
                for (int i = 0; i < this.sum_nodes-1; i++)
                {
                    if (this.nodes[i].is_reaf)
                    {
                        this.nodes[i].value = in_data[count];
                        count++;
                    }
                }
            }
            catch
            {
                Console.WriteLine("" + count);
            }

        }

        public void SetTReaf()
        {
            int count=0;
            t_reaf = new int[sum_reaves + 1];
            for (int i = 0; i < sum_nodes; i++)
            {
                if (nodes[i].is_reaf)
                {
                    t_reaf[count] = i;
                    count++;
                }
            }
        }
        public void WriteAll()
        {
            for (int i = 0; i < sum_nodes; i++)
            {
                if (nodes[i].is_reaf)
                {
                    WritePath(i);
                    Console.WriteLine(":" + nodes[i].value);
                }
            }
        }

        //public int 

        public void WritePath(int dst_node)
        {
            int posit = dst_node;
            int[] path = new int[nodes[dst_node].depth];

            for (int j=0; (j < (nodes[dst_node].depth)) ; j++)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (nodes[nodes[posit].parent].child[i] == posit)
                    {
                        posit = nodes[posit].parent;
                        path[j] = i;
                        break;
                    }
                }
            }
            for (int i = path.Length - 1; i >= 0; i--)
            {
                Console.Write("" + path[i]);
            }
            //Console.WriteLine();
        }

        
    }

    public class Node
    {
        public int[] child;
        public int value = -1;
        public int parent = -1;
        public int depth;
        public bool is_reaf = false;
        public Node()
        {
            this.child = new int[2];
            child[0] = child[1] = -1;
        }

    }
}
