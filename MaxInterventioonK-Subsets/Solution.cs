using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MaxInterventioonK_Subsets
{
    internal class Solution
    {
        private HashSet<int> _elements;
        private HashSet<int> _commonFeatures;
        private readonly Instance _instance;

        public Solution(Instance instance)
        {
            _elements = new HashSet<int>();
            _commonFeatures = new HashSet<int>();
            _instance = instance;
        }

        /// <summary>
        /// Adds an element to the solution.
        /// </summary>
        /// <param name="elementId">The ID of the element to add.</param>
        /// <returns>True if the element was added successfully; otherwise, false.</returns>
        public bool AddElement(int elementId)
        {
            if (!_elements.Add(elementId))
                return false;
                
            if (_elements.Count == 1)
            {
                _commonFeatures = new HashSet<int>(_instance.GetFeaturesByElement(elementId));
            }
            else
            {
                _commonFeatures.IntersectWith(_instance.GetFeaturesByElement(elementId));
            }

            return true;
        }

        /// <summary>
        /// Gets the count of elements in the solution.
        /// </summary>
        /// <returns>The count of elements in the solution.</returns>
        public int GetElementCount()
        {
            return _elements.Count;
        }

        /// <summary>
        /// Gets the maximum intersection value.
        /// </summary>
        /// <returns>The maximum intersection value.</returns>
        public int GetMaxInterrsection()
        {
            return _commonFeatures.Count;
        }

        /// <summary>
        /// Gets the common features in the solution.
        /// </summary>
        /// <returns>The common features in the solution.</returns>
        public IReadOnlyList<int> GetCommonFeatures()
        {
            return _commonFeatures.ToList();
        }

        /// <summary>
        /// Gets the elements in the solution.
        /// </summary>
        /// <returns>The elements in the solution.</returns>
        public IEnumerable<int> GetElements()
        {
            return _elements.ToList();
        }

        /// <summary>
        /// Gets the maximum intersection value with a specific element for this solution.
        /// </summary>
        /// <param name="e">The ID of the element.</param>
        /// <returns>The maximum intersection value with the specified element.</returns>
        public int GetMaxInterrsectionWithElement(int e)
        {
            HashSet<int> features = new HashSet<int>(_instance.GetFeaturesByElement(e));
            features.IntersectWith(_commonFeatures);
            return features.Count;
        }
    }
}
