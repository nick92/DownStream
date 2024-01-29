using DownStream.Models;

namespace DownStream.Services
{
    public class NodeServices(List<Branch> branches, List<Customer> customers)
    {
        private readonly List<Node> _nodes = new List<Node>();
        private readonly List<Branch> _branches = branches;
        private readonly List<NodeLink> _links = new List<NodeLink>();
        private readonly List<Customer> _customers = customers;

        public bool GenerateNodes(out string error)
        {
            error = string.Empty;

            try
            {
                bool isfirst = true;

                // loop through branches to create nodes and branches
                foreach (var branch in _branches)
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
            _links.Add(new NodeLink
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
        public List<Customer> QueryCustomersFromNode(int selectedNode, out string error)
        {
            error = string.Empty;
            var customers = new List<Customer>();

            try
            {
                var nodesToProcess = new Queue<Node>(_nodes.Where(node => node.Number == selectedNode).ToList());
                var processedNodes = new List<Node>();

                if (nodesToProcess.Count.Equals(0))
                {
                    error = "Selected node dones't exist, please enter a valid node id and try again";
                    return customers;
                }

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
                    var childBranches = _links.FindAll(link => link.ParentNode == currentNode.Id);
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
            catch (Exception ex)
            {
                error = ex.Message;
                return customers;
            }
        }
    }
}
