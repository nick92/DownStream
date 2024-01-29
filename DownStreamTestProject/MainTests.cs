using DownStream.Services;
using DownStream.Models;

namespace DownStreamTestProject
{
    [TestClass]
    public class DownSteamTests
    {
        [TestMethod]
        public void Return15CustomersSuccessTest()
        {
            var selectedNode = 10;

            var network = new Network
            {
                Branches = new List<Branch>
                {
                    new Branch
                    {
                        StartNode = 5,
                        EndNode = 10
                    },
                    new Branch
                    {
                        StartNode = 5,
                        EndNode = 20
                    },
                    new Branch
                    {
                        StartNode = 10,
                        EndNode = 30
                    },
                    new Branch
                    {
                        StartNode = 30,
                        EndNode = 40
                    },
                    new Branch
                    {
                        StartNode = 40,
                        EndNode = 50
                    },
                },
                Customers = new List<Customer>
                {
                    new Customer
                    {
                        Node = 5,
                        NumberOfCustomers = 1,
                    },
                    new Customer
                    {
                        Node = 30,
                        NumberOfCustomers = 10,
                    },
                    new Customer
                    {
                        Node = 40,
                        NumberOfCustomers = 5,
                    }
                },
            };
            string error = string.Empty;

            var nodeServices = new NodeServices(network.Branches, network.Customers);

            nodeServices.GenerateNodes(out error);

            var customers = nodeServices.QueryCustomersFromNode(selectedNode, out error);

            Assert.IsTrue(error.Equals(string.Empty));
            Assert.AreEqual(15, customers.Sum(c => c.NumberOfCustomers));
        }

        [TestMethod]
        public void CustomersMissingErrorTest()
        {
            var network = new Network
            {
                Branches = new List<Branch>
                {
                    new Branch
                    {
                        StartNode = 5,
                        EndNode = 10
                    },
                    new Branch
                    {
                        StartNode = 5,
                        EndNode = 20
                    },
                    new Branch
                    {
                        StartNode = 10,
                        EndNode = 30
                    },
                    new Branch
                    {
                        StartNode = 30,
                        EndNode = 40
                    },
                    new Branch
                    {
                        StartNode = 40,
                        EndNode = 50
                    },
                },
                Customers = new List<Customer>
                {
                   new Customer
                   {
                       Node = 90,
                       NumberOfCustomers = 1,
                   }
                },
            };
            string error = string.Empty;

            var nodeServices = new NodeServices(network.Branches, network.Customers);
            nodeServices.GenerateNodes(out error);
            Assert.IsTrue(error.Equals("Node missing for customer, please check data and re-submit"));
        }

        [TestMethod]
        public void InvalidNodeErrorTest()
        {
            var selectedNode = 100;

            var network = new Network
            {

                Branches = new List<Branch>
                {
                    new Branch
                    {
                        StartNode = 5,
                        EndNode = 10
                    },
                    new Branch
                    {
                        StartNode = 5,
                        EndNode = 20
                    },
                    new Branch
                    {
                        StartNode = 10,
                        EndNode = 30
                    },
                    new Branch
                    {
                        StartNode = 30,
                        EndNode = 40
                    },
                    new Branch
                    {
                        StartNode = 40,
                        EndNode = 50
                    },
                },
                Customers = new List<Customer>
                {
                   new Customer
                   {
                       Node = 90,
                       NumberOfCustomers = 1,
                   }
                },

            };
            string error = string.Empty;

            var nodeServices = new NodeServices(network.Branches, network.Customers);
            nodeServices.GenerateNodes(out error);
            var customers = nodeServices.QueryCustomersFromNode(selectedNode, out error);

            Assert.IsTrue(error.Equals("Selected node dones't exist, please enter a valid node id and try again"));
        }
    }
}