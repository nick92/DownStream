using DownStream.Controllers;

namespace DownStream.Models
{
    public class Request
    {
        public Network Network { get; set; }
        public int SelectedNode { get; set; }
    }

    public class Network
    {
        public List<Branch> Branches { get; set; }

        public List<Customer> Customers { get; set; }
    }

    public class Node
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public bool isRootNode { get; set; }
        public Customer Customer { get; set; }
    }

    public class NodeLink
    {
        public Guid Id { get; set; }
        public Guid ParentNode { get; set; }
        public Guid ChildNode { get; set; }
    }

    public class Branch
    {
        public int StartNode { get; set; }
        public int EndNode { get; set; }
    }

    public class Customer
    {
        public int NumberOfCustomers { get; set; }
        public int Node { get; set; }
    }

    public class Response
    {
        public List<Customer> DownStreamCustomers { get; set; }
        public int DownStreamCutomerCount { get { return DownStreamCustomers.Sum(c => c.NumberOfCustomers); } }
        public ErrorDetails Errors { get; set; }
    }
}
