﻿/*
 * This code implements priority queue which uses min-heap as underlying storage
 * 
 * Copyright (C) 2010 Alexey Kurakin
 * www.avk.name
 * alexey[ at ]kurakin.me
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TSP
{
    //
    // CREDIT: http://stackoverflow.com/a/4994931/1457934
    //
    class PriorityQueue
    {
        int total_size;
        SortedDictionary<double, Queue> storage;

        public PriorityQueue()
        {
            this.storage = new SortedDictionary<double, Queue>();
            this.total_size = 0;
        }

        public bool IsEmpty
        {
          get { return (total_size == 0); }
        }

        public TSPState Dequeue()
        {
            if (IsEmpty)
            {
                throw new Exception("Please check that priorityQueue is not empty before dequeing");
            }
            else
            {
                var kv = storage.First();
                Queue q = kv.Value;
                TSPState deq = (TSPState)q.Dequeue();

                if (q.Count == 0)
                    storage.Remove(kv.Key);

                total_size--;

                return deq;
            }
        }

        public void Enqueue(TSPState item)
        {
            double prio = item.Priority();

            if (!storage.ContainsKey(prio))
            {
                storage.Add(prio, new Queue());
            }
            storage[prio].Enqueue(item);
            total_size++;
        }

        public void Prune(double pruneLimit)
        {
          for(int i = 0; i < storage.Keys.Count; i++) {
            var key = storage.Keys.ElementAt(i);
            if (key >= pruneLimit) {
              // Decrement the PQ size
              total_size -= storage[key].Count;
              // Remove the Queue that's too largee
              storage.Remove(key);
            }
          }
        }

        // TODO: Make me optimized
        public bool Contains(TSPState state) 
        {
            double prio = state.Priority();
            if (!storage.ContainsKey(prio))
            {
                return false;
            }
            else
            {
                return storage[prio].Contains(state);
            }
        }

    }
}
