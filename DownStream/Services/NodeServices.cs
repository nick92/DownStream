﻿using DownStream.Models;

namespace DownStream.Services
{
    public class NodeServices
    {
        private List<Node> _nodes = [];
        private List<NodeLink> _branches = [];
        private List<Customer> _customers = [];

        public bool GenerateNodes(List<Branch> branches, List<Customer> customers, out string error)
        {
            error = string.Empty;
            _customers = customers;

            try
            {
                bool isfirst = true;

                // loop through branches to create nodes and branches
                foreach (var branch in branches)
                {
                    // check if exists and add parent node
                    var parentNode = GetOrCreateNode(branch.StartNode, isfirst);

                    // check if exists and add child node
                    var childNode = GetOrCreateNode(branch.EndNode, isfirst);

                    // create child / parent links between nodes
                    CreateNodeLink(parentNode.Id, childNode.Id);
                }

                // ensure all customers have been assigned a node, error if not
                if (!EnsureCustomerNodeExists())
                {
                    error = "Node missing for customer, please check data and re-submit";
                    return false;
                }

            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }

            return true;
        }

        // Return node if exists or create new one
        private Node GetOrCreateNode(int nodeId, bool isFirst)
        {
            return _nodes.Find(node => node.Number.Equals(nodeId)) ?? AddNodeIfDoesntExist(nodeId, isFirst);
        }

        // Add a node if it does not exist in _nodes
        private Node AddNodeIfDoesntExist(int nodeId, bool isFirst)
        {
            var customer = _customers.Find(cust => cust.Node.Equals(nodeId));
            var node = new Node
            {
                Id = Guid.NewGuid(),
                Number = nodeId,
                isRootNode = isFirst,
                Customer = customer
            };
            _nodes.Add(node);
            return node;
        }

        // add link between 2 nodes
        private void CreateNodeLink(Guid parentId, Guid childId)
        {
            _branches.Add(new NodeLink
            {
                Id = Guid.NewGuid(),
                ParentNode = parentId,
                ChildNode = childId,
            });
        }

        private bool EnsureCustomerNodeExists()
        {
            foreach (var customer in _customers)
            {
                if (!_nodes.Exists(node => node.Number.Equals(customer.Node)))
                    return false;
            }

            return true;
        }

        // Query customers from given node
        public List<Customer> QueryCustomersFromNode(int selectedNode)
        {
            var customers = new List<Customer>();
            var nodesToProcess = new Queue<Node>(_nodes.FindAll(node => node.Number == selectedNode));
            var processedNodes = new List<Node>();

            // process through all found nodes to accumulate customer count
            while (nodesToProcess.Count > 0)
            {
                var currentNode = nodesToProcess.Dequeue();
                if (processedNodes.Contains(currentNode))
                {
                    continue;
                }
                // add to processed nodes 
                processedNodes.Add(currentNode);

                // query out customer against node if exists
                if (currentNode.Customer != null)
                {
                    customers.Add(currentNode.Customer);
                }

                // query branches associated to node
                var childBranches = _branches.FindAll(branch => branch.ParentNode == currentNode.Id);
                foreach (var childBranch in childBranches)
                {
                    // query all nodes associated to branch
                    var childNodes = _nodes.FindAll(node => node.Id == childBranch.ChildNode);
                    foreach (var childNode in childNodes)
                    {
                        // check if node has been processed, if not add for processing
                        if (!processedNodes.Contains(childNode))
                        {
                            nodesToProcess.Enqueue(childNode);
                        }
                    }
                }
            }


            return customers;
        }
    }
}
