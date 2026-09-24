using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MaxInterventionK_Subsets
{
    internal class Solution
    {
        private HashSet<int> _elements;
        private BitSet _commonFeatures;
        private readonly Instance _instance;

        public Solution(Instance instance)
        {
            _elements = new HashSet<int>();
            _instance = instance;
            _commonFeatures = new BitSet(_instance.GetFeatureCount());
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
                _commonFeatures = _instance.GetFeaturesBitSetByElement(elementId);
            }
            else
            {
                _commonFeatures.AndWith(_instance.GetFeaturesBitSetByElement(elementId));
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
            return _commonFeatures.Count();
        }

        /// <summary>
        /// Gets the common features in the solution.
        /// </summary>
        /// <returns>The common features in the solution.</returns>
        public IReadOnlyList<int> GetCommonFeatures()
        {
            return _commonFeatures.GetIdList();
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
            BitSet actualFeatures = _commonFeatures.Clone();
            BitSet elementFeatures = _instance.GetFeaturesBitSetByElement(e);

            actualFeatures.AndWith(elementFeatures);
            return actualFeatures.Count();
        }
    }
}
