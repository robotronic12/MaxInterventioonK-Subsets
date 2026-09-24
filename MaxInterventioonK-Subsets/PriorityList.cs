using System;
using System.Collections.Generic;

namespace MaxInterventionK_Subsets
{
    internal sealed class PriorityCandidate<T>
    {
        public T ElementId { get; private set; }

        public int Priority { get; private set; }

        public long InsertionOrder { get; private set; }

        public PriorityCandidate(
            T elementId,
            int priority,
            long insertionOrder)
        {
            ElementId = elementId;
            Priority = priority;
            InsertionOrder = insertionOrder;
        }
    }

    internal sealed class PriorityList<T>
    {
        private readonly SortedSet<PriorityCandidate<T>> _items;
        private long _nextInsertionOrder;

        public PriorityList()
        {
            _items = new SortedSet<PriorityCandidate<T>>(
                Comparer<PriorityCandidate<T>>.Create(
                    (first, second) =>
                    {
                        // Prioridad descendente.
                        int priorityComparison =
                            second.Priority.CompareTo(first.Priority);

                        if (priorityComparison != 0)
                        {
                            return priorityComparison;
                        }

                        // Misma prioridad: orden de inserción.
                        return first.InsertionOrder.CompareTo(
                            second.InsertionOrder);
                    }));
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public void Add(T elementId, int priority)
        {
            PriorityCandidate<T> candidate =
                new PriorityCandidate<T>(
                    elementId,
                    priority,
                    _nextInsertionOrder);

            _nextInsertionOrder++;
            _items.Add(candidate);
        }

        public PriorityCandidate<T> Peek()
        {
            if (_items.Count == 0)
            {
                throw new InvalidOperationException(
                    "La lista de prioridades está vacía.");
            }

            return _items.Min;
        }

        public PriorityCandidate<T> Remove()
        {
            PriorityCandidate<T> candidate = Peek();
            _items.Remove(candidate);
            return candidate;
        }
    }
}