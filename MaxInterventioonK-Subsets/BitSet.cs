using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxInterventionK_Subsets
{
    internal class BitSet
    {
        private const int BITS_PER_WORD = 64;

        private readonly ulong[] _words;
        private readonly int _bitsCount;

        public BitSet(int bitsCount)
        {
            if (bitsCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bitsCount));
            }

            _bitsCount = bitsCount;

            int wordCount = (bitsCount + BITS_PER_WORD - 1) / BITS_PER_WORD;

            _words = new ulong[wordCount];
        }

        private BitSet(int bitsCount, ulong[] words)
        {
            _bitsCount = bitsCount;
            _words = words;
        }

        /// <summary>
        /// Sets the bit with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the bit to set.</param>
        public void Set(int id)
        {
            ValidateId(id);

            int wordIndex = id / BITS_PER_WORD;
            int bitIndex = id % BITS_PER_WORD;

            _words[wordIndex] |= 1UL << bitIndex;
        }

        /// <summary>
        /// Clears the bit with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the bit to clear.</param>
        public void Clear(int id)
        {
            ValidateId(id);

            int wordIndex = id / BITS_PER_WORD;
            int bitIndex = id % BITS_PER_WORD;

            _words[wordIndex] &= ~(1UL << bitIndex);
        }

        /// <summary>
        /// Checks if the bit with the specified ID is set.
        /// </summary>
        /// <param name="id">The ID of the bit to check.</param>
        /// <returns>True if the bit is set; otherwise, false.</returns>
        public bool Contains(int id)
        {
            ValidateId(id);

            int wordIndex = id / BITS_PER_WORD;
            int bitIndex = id % BITS_PER_WORD;

            return (_words[wordIndex] & (1UL << bitIndex)) != 0;
        }

        /// <summary>
        /// Performs a bitwise AND operation with another BitSet.
        /// </summary>
        /// <param name="other">The other BitSet to perform the AND operation with.</param>
        /// <exception cref="ArgumentNullException">Thrown if the other BitSet is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the other BitSet has a different size.</exception>
        public void AndWith(BitSet other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (_bitsCount != other._bitsCount)
            {
                throw new ArgumentException(
                    "The bit sets must have the same size.");
            }

            for (int i = 0; i < _words.Length; i++)
            {
                _words[i] &= other._words[i];
            }
        }

        /// <summary>
        /// Performs a bitwise OR operation with another BitSet.
        /// </summary>
        /// <param name="other">The other BitSet to perform the OR operation with.</param>
        /// <exception cref="ArgumentNullException">Thrown if the other BitSet is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the other BitSet has a different size.</exception>
        public void OrWith(BitSet other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            if (_bitsCount != other._bitsCount)
            {
                throw new ArgumentException(
                    "The bit sets must have the same size.");
            }
            for (int i = 0; i < _words.Length; i++)
            {
                _words[i] |= other._words[i];
            }
        }

        /// <summary>
        /// Creates a copy of the current BitSet.
        /// </summary>
        /// <returns>A new BitSet that is a copy of the current BitSet.</returns>
        public BitSet Clone()
        {
            ulong[] wordsCopy =
                new ulong[_words.Length];

            Array.Copy(
                _words,
                wordsCopy,
                _words.Length);

            return new BitSet(
                _bitsCount,
                wordsCopy);
        }

        /// <summary>
        /// Counts the number of bits that are set to 1.
        /// </summary>
        /// <returns>The number of bits that are set to 1.</returns>
        public int Count()
        {
            int count = 0;

            foreach (ulong word in _words)
            {
                ulong value = word;

                while (value != 0)
                {
                    value &= value - 1;
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Creates a list of IDs for the bits that are set to 1.
        /// </summary>
        /// <returns>A read-only list of IDs for the bits that are set to 1.</returns>
        public IReadOnlyList<int> GetIdList()
        {
            List<int> idList =
                new List<int>();

            for (int i = 0; i < _bitsCount; i++)
            {
                if (Contains(i))
                {
                    idList.Add(i);
                }
            }

            return idList;
        }

        private void ValidateId(int id)
        {
            if (id < 0 ||
                id >= _bitsCount)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(id));
            }
        }
    }
}
