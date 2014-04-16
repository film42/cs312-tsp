using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace TSP
{
    class PriorityQueue
    {
        int total_size;
        SortedDictionary<double, Queue> storage;

        public PriorityQueue()
        {
            this.storage = new SortedDictionary<double, Queue>();
            this.total_size = 0;
        }

        public int size
        {
            get { return total_size; }
        }

        public bool IsEmpty()
        {
            // Check if empty: based on total_size because keys can have multiple states
            return (total_size == 0);
        }

        public TSPState Dequeue()
        {
            if (IsEmpty())
            {
                throw new Exception("Please check that priorityQueue is not empty before dequeing");
            }
            else
            {
                // Get first item from the sorted dictionary
                var kv = storage.First();
                // Get the Queue from the val
                Queue q = kv.Value;
                // Then grab the first item from it
                TSPState deq = (TSPState)q.Dequeue();
                
                // Remove if now empty
                if (q.Count == 0)
                    storage.Remove(kv.Key);
                
                // Decrement the total size
                total_size--;
                
                // Return the state that's been dequeued
                return deq;
            }
        }

        public void Enqueue(TSPState item, double prio)
        {
            // Check if key prio exists
            if (!storage.ContainsKey(prio))
            {
                // Add a new queue for key prio
                storage.Add(prio, new Queue());
            }
            // Enqueue state at prio in queue
            storage[prio].Enqueue(item);
            // Inc the total size of the agenda
            total_size++;
        }

        public bool Contains(TSPState state)
        {
            // Get the state's priority for lookup
            double prio = state.Bound;
            // If it doesn't exist, absolutely false
            if (!storage.ContainsKey(prio))
            {
                return false;
            }
            else
            {
                // Otherwise run contains on the inner Queue
                return storage[prio].Contains(state);
            }
        }
    }
}
