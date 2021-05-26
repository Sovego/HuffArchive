using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata.Ecma335;
// C:\Users\egors\RiderProjects\HuffArchive\HuffArchive\Resources\test.txt

namespace HuffArchive
{
    class Program
    {
        public static byte Delta=0;
        public static Node RecoverTree(Node head, BinaryReader sr)
        {
            Node newLeft = new Node();
            Node newRight = new Node();
            if (Convert.ToChar(sr.ReadChar()) == '0')
            {
                head.Left = newLeft;
                RecoverTree(head.Left, sr);
                head.Right = newRight;
                RecoverTree(head.Right, sr);
            }
            else
            {
                head.Symbol = Convert.ToChar(sr.ReadChar());
            }
            return head;
        }
        private static string EncTree(Node temp)
        {
            if (temp.IsLeaf(temp)) return "1" + temp.Symbol;
            return "0" + EncTree(temp.Left) + EncTree(temp.Right);
        }
        static void Main(string[] args)
        {
            string input;
            string pathtofile;
            Console.WriteLine("Input path to file");
            HuffmanTree huffmanTree = new HuffmanTree();
            pathtofile=Console.ReadLine();
            using (StreamReader sr = new StreamReader(pathtofile))
            {
                input = sr.ReadToEnd();
            }
            //HuffmanTree huffmanTree = new HuffmanTree();
            // Build the Huffman tree
            huffmanTree.Build(input);
            // Encode
            BitArray encoded = huffmanTree.Encode(input);
            List<byte> bytes = new List<byte>();
            Console.Write("Encoded ");
            string bytestr = "";
            int i=0;
            while(i<encoded.Length)
            {
                if (bytestr.Length < 8)
                {
                    bytestr += encoded[i] ? 1 : 0.ToString();
                    i++;
                }
                else
                {
                    bytes.Add(Convert.ToByte(bytestr,2));
                    bytestr = "";
                }
            }
            
            if (bytestr.Length <= 8)
            {
                while (bytestr.Length < 8)
                {
                    bytestr += "0";
                    Delta++;
                }
                bytes.Add(Convert.ToByte(bytestr,2));
            }
            using (StreamWriter sw = new StreamWriter("archive.txt",false))
            {
                sw.Write(EncTree(huffmanTree.Root));
            }

            using (FileStream fs = new FileStream("archive.txt", FileMode.Append))
            {
                //fs.Position = fs.Length-1;
                fs.Write(bytes.ToArray());
                fs.WriteByte(Delta);
            }
            Console.WriteLine();
 
            // Decode
            huffmanTree.Root = new Node();
            //BinaryReader ssr = new BinaryReader(new FileStream("archive.txt",FileMode.Open));
                
           // ssr.Close();
            huffmanTree.Decode();
            Console.WriteLine("Decoded");
            //Console.WriteLine("Decoded: " + decoded);
            Console.WriteLine("All done");
            Console.ReadLine();
        }
    }
}