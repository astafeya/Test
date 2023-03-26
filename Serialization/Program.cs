using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Serialization
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"D:\ListRandSave.txt";
            ListRand listRand = CreateListRand();
            using (FileStream s = new FileStream(path, FileMode.Create)) {
                listRand.Serialize(s);
            }
            PrintListRand(listRand);
            Console.WriteLine();

            ListRand newRand = new ListRand();
            using (FileStream s = File.OpenRead(path))
            {
                newRand.Deserialize(s);
            }
            PrintListRand(newRand);
            Console.ReadLine();
        }

        static ListRand CreateListRand()
        {
            ListNode LN1 = new ListNode();
            ListNode LN2 = new ListNode();
            ListNode LN3 = new ListNode();
            ListNode LN4 = new ListNode();
            ListNode LN5 = new ListNode();

            LN1.Next = LN2;
            LN1.Data = "ListNode1";

            LN2.Prev = LN1;
            LN2.Next = LN3;
            LN2.Rand = LN2;
            LN2.Data = "ListNode2";

            LN3.Prev = LN2;
            LN3.Next = LN4;
            LN3.Rand = LN5;
            LN3.Data = "ListNode3";

            LN4.Prev = LN3;
            LN4.Next = LN5;
            LN4.Rand = LN1;
            LN4.Data = "ListNode4";

            LN5.Prev = LN4;
            LN5.Data = "ListNode5";

            ListRand listRand = new ListRand();
            listRand.Count = 5;
            listRand.Head = LN1;
            listRand.Tail = LN5;

            return listRand;
        }

        static void PrintListRand(ListRand listRand)
        {
            Console.WriteLine("ListRand {");
            Console.WriteLine("  Count: " + listRand.Count);
            if (listRand.Count == 0)
            {
                Console.WriteLine("}");
                return;
            }
            ListNode current = listRand.Head;
            int i = 0;
            do
            {
                PrintListNode(current);
                current = current.Next;
            } while (current != null);
            Console.WriteLine("}");
        }

        static void PrintListNode(ListNode listNode)
        {
            Console.WriteLine("  ListNode {");
            Console.WriteLine("    Rand = " + ((listNode.Rand == null) ? "null" : listNode.Rand.Data));
            Console.WriteLine("    Data = " + listNode.Data);
            Console.WriteLine("  }");
        }

    }

    class ListNode
    {
        public ListNode Prev;
        public ListNode Next;
        public ListNode Rand; // произвольный элемент внутри списка
        public string Data;
    }


    class ListRand
    {
        public ListNode Head;
        public ListNode Tail;
        public int Count;

        public void Serialize(FileStream s)
        {
            AddLine(s, Count.ToString());
            if (Count == 0) return;
            Dictionary<ListNode, int> dict = new Dictionary<ListNode, int>();
            ListNode current = Head;
            for (int i = 0; i < Count; i++)
            {
                dict.Add(current, i);
                current = current.Next;
            }
            current = Head;
            for (int i = 0; i < Count; i++)
            {
                AddLine(s, "\n");
                AddLine(s, current.Data);
                AddLine(s, "\n");

                string rand;
                if (current.Rand == null)
                    rand = "null";
                else if (current.Rand == current)
                    rand = "self";
                else 
                {
                    dict.TryGetValue(current.Rand, out int r);
                    rand = r.ToString();
                }
                AddLine(s, rand);
                current = current.Next;
            }
        }

        public void Deserialize(FileStream s)
        {
            string[] strings = GetStrings(s);
            Count = int.Parse(strings[0]);
            if (Count == 0) return;
            ListNode[] listNodes = new ListNode[Count];
            int[] randNodes = new int[Count];
            for (int i = 0; i < Count; i++)
            {
                listNodes[i] = new ListNode();
                listNodes[i].Data = strings[1 + i * 2];
                randNodes[i] = -1;
                if (strings[2 + i * 2] != "null")
                {
                    if (strings[2 + i * 2] == "self") listNodes[i].Rand = listNodes[i];
                    else randNodes[i] = int.Parse(strings[2 + i * 2]);
                }
            }
            for (int i = 0; i < Count; i++)
            {
                if (i > 0) listNodes[i].Prev = listNodes[i - 1];
                if (i < Count - 1) listNodes[i].Next = listNodes[i + 1];
                if (randNodes[i] != -1)
                {
                    listNodes[i].Rand = listNodes[randNodes[i]];
                }
            }
            Head = listNodes[0];
            Tail = listNodes[Count - 1];
        }

        private void AddLine(FileStream s, string text)
        {
            byte[] byteArray = new UTF8Encoding(true).GetBytes(text);
            s.Write(byteArray, 0, byteArray.Length);
        }

        private string[] GetStrings(FileStream s)
        {
            byte[] bytes = new byte[s.Length];
            int length = s.Read(bytes, 0, bytes.Length);
            return new UTF8Encoding(true).GetString(bytes, 0, length).Split('\n');

        }
    }

}
