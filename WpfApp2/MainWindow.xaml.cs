using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
                var listNodes = new List<ListNode>();
                var temp = Head;
                do
                {
                    listNodes.Add(temp);
                    temp = temp.Next;
                } while (temp != null);

                using var writer = new StreamWriter(s);
                foreach (var nodes in listNodes)
                    writer.WriteLine(nodes.Data + ":" + listNodes.IndexOf(nodes.Rand));
            }

            public void Deserialize(FileStream s)
            {
                var listNodes = new List<ListNode>();
                var temp = new ListNode();
                Count = 0;
                Head = temp;
                try
                {
                    using (var sr = new StreamReader(s))
                    {
                        string line;
                        while ((line = sr.ReadLine()!) != null)
                        {
                            if (line.Equals(""))
                                continue;
                            Count++;
                            temp.Data = line;
                            var next = new ListNode();
                            temp.Next = next;
                            listNodes.Add(temp);
                            next.Prev = temp;
                            temp = next;
                        }
                    }
                    Tail = temp.Prev;
                    Tail.Next = null!;

                    foreach (var nodes in listNodes)
                    {
                        nodes.Rand = listNodes[Convert.ToInt32(nodes.Data.Split(':')[1])];
                        nodes.Data = nodes.Data.Split(':')[0];
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Не удалось обработать файл данных, возможно, он поврежден, подробности:" + "\n" + e.Message + "\nНажмите любую клавишу для выхода");
                    Console.Read();
                    Environment.Exit(0);
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            MessageBox.Show("Для работы введите в TextBox количество нодов для генерации");
        }
        private static readonly Random Rand = new();

        private static ListNode AddNode(ListNode prev)
        {
            var result = new ListNode
            {
                Prev = prev,
                Next = null!,
                Data = Rand.Next(0, 100).ToString()
            };
            prev.Next = result;
            return result;
        }

        private static ListNode CreateRandomNode(ListNode head, int length)
        {
            var rand = Rand.Next(0, length);
            var i = 0;
            var result = head;
            while (i < rand)
            {
                result = result.Next;
                i++;
            }

            return result;
        }
        public static int length;

        public void Выполнить_Click(object sender, RoutedEventArgs e)
        {
                var head = new ListNode
                {
                    Data = Rand.Next(0, 500).ToString()
                };
                var tail = head;

                for (var i = 1; i < length; i++)
                    tail = AddNode(tail);

                var temp = head;

                for (var i = 0; i < length; i++)
                {
                    temp.Rand = CreateRandomNode(head, length);
                    temp = temp.Next;
                }

                var first = new ListRand
                {
                    Head = head,
                    Tail = tail,
                    Count = length
                };

                try
                {
                    var stream = new FileStream("List.txt", FileMode.Create);
                    first.Serialize(stream);

                    var second = new ListRand();
                    stream = new FileStream("List.txt", FileMode.Open);
                    second.Deserialize(stream);

                if (second.Tail.Data == first.Tail.Data)
                {
                    MessageBox.Show("Успех");
                }
                Результат.Text = "";
                var tempp = second.Head;

                for(var i = 0; i < second.Count; i++)
                {
                    Результат.Text += tempp.Data + ":" + tempp.Rand.Data + "\n";
                    temp = tempp.Next;
                    tempp = temp;

                }
                    Console.Read();

                }
                catch (Exception f)
                {
                    MessageBox.Show(f.Message + "\nНажмите любую клавишу для выхода");
                    Console.Read();
                    Environment.Exit(0);
                }
            }

        private void Сохранить_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(Длина.Text))
            {
                MessageBox.Show("Поле ввода пустое. Введите количество нодов.");
            }
            else
            {
                length = Convert.ToInt32(Длина.Text);
                MessageBox.Show("Количество нодов изменено");
                Выполнить.IsEnabled = true;
                Длина.Text = "";
            }
        }

        private void Длина_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
        private void ДлинаCheck(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
    }
