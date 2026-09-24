using System;
using System.Collections.Generic;
using System.IO;

namespace MaxInterventioonK_Subsets
{
    internal class Instance
    {

        private int _k;
        private  int _elementCount;
        private  int _featureCount;
        private  int _edgeCount;
        private  List<int>[] _featuresByElement;

        public Instance(string filePath)
        {
            try
            {
                LoadInstanceFromFile(filePath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading instance from file: {ex.Message}");
            }
        }

        #region Public Methods
        /// <summary>
        /// Gets the value of k for the instance.
        /// </summary>
        /// <returns>The K value</returns>
        public int GetK()
        {
            return _k;
        }

        /// <summary>
        /// Gets the number of elements in the instance.
        /// </summary>
        /// <returns>The number of elements in the instance.</returns>
        public int GetElementCount()
        {
            return _elementCount;
        }

        /// <summary>
        /// Gets the number of features in the instance.
        /// </summary>
        /// <returns>The number of features in the instance.</returns>
        public int GetFeatureCount()
        {
            return _featureCount;
        }

        /// <summary>
        /// Gets the number of edges in the instance.
        /// </summary>
        /// <returns>The number of edges in the instance.</returns>
        public int GetEdgeCount()
        {
            return _edgeCount;
        }

        /// <summary>
        /// Retrieves a read-only list of feature identifiers associated with the specified element.
        /// </summary>
        /// <param name="elementId">The identifier of the element for which features are to be retrieved.</param>
        /// <returns>A read-only list of feature identifiers.</returns>
        public IReadOnlyList<int> GetFeaturesByElement(int elementId)
        {
            return _featuresByElement[elementId].AsReadOnly();
        }

        /// <summary>
        /// Writes the instance data to the console for debugging purposes.
        /// </summary>
        public void WriteInConsole()
        {
            Console.WriteLine($"Element Count: {_elementCount}");
            Console.WriteLine($"Feature Count: {_featureCount}");
            Console.WriteLine($"Edge Count: {_edgeCount}");

            for (int elementId = 0; elementId < _elementCount; elementId++)
            {
                Console.WriteLine($"Element {elementId}: Features [{string.Join(", ", _featuresByElement[elementId])}]");
            }
        }
        #endregion

        #region private methods
        private void LoadInstanceFromFile(string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                int[] header = ReadLineValues(reader);

                _elementCount = header[0];
                _featureCount = header[1];
                _edgeCount = header[2];
                _k = header[3];

                _featuresByElement = new List<int>[_elementCount];

                for (int i = 0; i < _elementCount; i++)
                {
                    _featuresByElement[i] = new List<int>();
                }

                for (int i = 0; i < _edgeCount; i++)
                {
                    int[] edge = ReadLineValues(reader);

                    int elementId = edge[0] - 1;
                    int featureId = edge[1] - 1;

                    _featuresByElement[elementId].Add(featureId);
                }
            }
        }

        private static int[] ReadLineValues(StreamReader reader)
        {
            string line = reader.ReadLine();

            if (string.IsNullOrWhiteSpace(line))
            {
                throw new FormatException("La línea está vacía.");
            }

            string[] values = line.Split(
                new[] { ' ', '\t' },
                StringSplitOptions.RemoveEmptyEntries);

            int[] result = new int[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                result[i] = int.Parse(values[i]);
            }

            return result;
        }
        #endregion
    }
}