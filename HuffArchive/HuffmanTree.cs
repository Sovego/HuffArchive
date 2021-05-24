using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HuffArchive
{
    public class HuffmanTree
    {
        private Dictionary<string, char> EncodeTable = new Dictionary<string, char>();
        private List<Node> nodes = new List<Node>();
        public Node Root { get; set; }
        public Dictionary<char, int> Frequencies = new Dictionary<char, int>();
        private static object locker;
        

        public void Build(string source)
        {
            for (int i = 0; i < source.Length; i++)
            {
                if (!Frequencies.ContainsKey(source[i]))
                {
                    Frequencies.Add(source[i], 0);
                }

                Frequencies[source[i]]++;
            }

            foreach (KeyValuePair<char, int> symbol in Frequencies)
            {
                nodes.Add(new Node() {Symbol = symbol.Key, Frequency = symbol.Value});
            }

            while (nodes.Count > 1)
            {
                List<Node> orderedNodes = nodes.OrderBy(node => node.Frequency).ToList<Node>();

                if (orderedNodes.Count >= 2)
                {
                    // Take first two items
                    List<Node> taken = orderedNodes.Take(2).ToList<Node>();

                    // Create a parent node by combining the frequencies
                    Node parent = new Node()
                    {
                        Symbol = '*',
                        Frequency = taken[0].Frequency + taken[1].Frequency,
                        Left = taken[0],
                        Right = taken[1]
                    };

                    nodes.Remove(taken[0]);
                    nodes.Remove(taken[1]);
                    nodes.Add(parent);
                }

                this.Root = nodes.FirstOrDefault();

            }

        }

        public BitArray Encode(string source)
        {
            List<bool> encodedSource = new List<bool>();

            for (int i = 0; i < source.Length; i++)
            {
                List<bool> encodedSymbol = this.Root.TreeTrip(source[i], new List<bool>());
                encodedSource.AddRange(encodedSymbol);
            }

            BitArray bits = new BitArray(encodedSource.ToArray());

            return bits;
        }

        void FillEncodingArray(Node node, String codeBefore, String direction)
        {
            //заполнить кодировочную таблицу
            if (node.IsLeaf(node))
            {
                EncodeTable.Add(codeBefore+direction,node.Symbol);
            }
            else
            {
                FillEncodingArray(node.Left, codeBefore + direction, "0");
                FillEncodingArray(node.Right, codeBefore + direction, "1");
            }
        }
        public void Decode()
        {
            
            FillEncodingArray(Root, "", "");
            string decoded = "";
            string tmpstr = "";
            string compress = "";
            string bytefile;
            StreamWriter sw = new StreamWriter("unarchive.txt", false);
            BinaryReader fssStream = new BinaryReader(new FileStream("archive.txt", FileMode.Open),Encoding.ASCII);
            byte endbyte = 0;
            while (Convert.ToChar(endbyte) != '\n')
            {
                endbyte = (byte) fssStream.ReadByte();
            }
            while (fssStream.BaseStream.Position < fssStream.BaseStream.Length-1)
            {
                bytefile = Convert.ToString((int)fssStream.ReadByte(), 2);
                string deltastring = "00000000";
                int index = deltastring.Length - bytefile.Length;
               deltastring = deltastring.Remove(index, bytefile.Length);
               deltastring = deltastring.Insert(index, bytefile);
               bytefile = deltastring;
               if (fssStream.BaseStream.Position + 1 == fssStream.BaseStream.Length)
                {
                    int delta = fssStream.ReadByte();
                    bytefile = bytefile.Remove(bytefile.Length - delta, delta);
                }
                compress = bytefile;
                //lock (locker)
               // {
                    for (int i = 0; i < compress.Length; i++)
                    {
                        tmpstr += compress[i];
                        if (EncodeTable.ContainsKey(tmpstr))
                        {
                            sw.Write(EncodeTable[tmpstr]);
                            tmpstr = "";
                        }
                    }
              //  }
            }
            //return decoded;
        }
        public bool IsLeaf(Node node)
        {
            return (node.Left == null && node.Right == null);
        }
    }
}
        
    

