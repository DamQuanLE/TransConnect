using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransConnect.DbContexts;
using TransConnect.Models;

namespace TransConnect.ViewModels
{
    public class Node
    {
        public Employee Employee { get; set; }
        public Node? Parent { get; set; }


        public ObservableCollection<Node> Items { get; set; }

        public Node()
        {
            Items = new ObservableCollection<Node>();
        }

        public override string ToString()
        {
            return Employee.ToString();
        }
    }

    public class NArayTree
    {
        public Node root { get; set; }

        public static NArayTree createTree()
        {
            List<Employee> employees;
            using (var context = new TransConnectDbContext())
            {
                employees = context.Employees.Include("Manager").ToList();
            }

            var tree = new NArayTree();

            Dictionary<Node, Employee?> nodes = new Dictionary<Node, Employee?>();
            foreach (var e in employees)
            {
                Node node = new Node();
                node.Employee = e;
                nodes[node] = e.Manager;
            }

            foreach (var node in nodes)
            {
                if (node.Value == null)
                {
                    tree.root = node.Key;
                }
                else
                {
                    // Node la nhan vien dang xet, Value la nhan vien quan ly cua node
                    foreach (var n in nodes)
                    {
                        // Tim kiem node cua nhan vien quan ly (n la node cua nhan vien quan ly)
                        if (n.Key.Employee == node.Value)
                        {
                            n.Key.Items.Add(node.Key);
                            node.Key.Parent = n.Key;
                        }
                    }
                }
            }

            return tree;
        }
    }



    public class EmployeeTreeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private NArayTree _tree { get; set; }

        private ObservableCollection<Node> _heads = new ObservableCollection<Node>();

        public ObservableCollection<Node> Heads
        {
            get { return _heads; }
            set
            {
                _heads = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Heads"));
            }
        }
        
        public EmployeeTreeViewModel()
        {
            _tree = NArayTree.createTree();
            Heads.Add(_tree.root);
        }
    }
}
