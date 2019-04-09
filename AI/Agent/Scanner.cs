using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CJS.AI
{
    public class Scanner : MonoBehaviour
    {
        public LayerMask layerMask;

        public bool IsFindElement { get { return elementInRange.Count > 0; } }
        public bool IsFindPlayer { get { return player != null; } }

        public event System.Action TargetChange;
        protected Player player;
        protected List<Element> elementInRange = new List<Element>();
        protected Element targetElement;

        
        private void OnTriggerEnter2D(Collider2D collision)
        {
            
            if ((1 << collision.gameObject.layer & layerMask.value) == 0)
            {
                return;
            }
            Element element = collision.GetComponent<Element>();

            if (!element.is_Connected)
            {
                element.OnConnect.AddListener(TargetConnected);
                
                elementInRange.Add(element);
            }
            else
            {
                player = GameManager.Instance.playList[element.PlayerID];

            }
                           
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if ((1 << collision.gameObject.layer & layerMask.value) == 0)
            {
                return;
            }
            Element element = collision.GetComponent<Element>();
            if (!element.is_Connected)
            {
                //element.OnConnect.RemoveListener(TargetConnected);
                //elementInRange.Remove(element);
            }
            else
            {
                player = null;

            }
        }
        protected virtual Player GetPlayer()
        {
            return player;
        }
        public virtual Element GetBestTarget()
        {
            int length = elementInRange.Count;

            if (length == 0)
            {
                return null;
            }

            Element best = null;
            float distance = float.MaxValue;
            for (int i = length - 1; i >= 0; i--)
            {
                Element element = elementInRange[i];
               
                float currentDistance = Vector3.Distance(transform.position, element.transform.position);
                if (currentDistance < distance)
                {
                    distance = currentDistance;
                    best = element;
                }
            }
            targetElement = best;
            return best;
        }

        private void TargetConnected(Element element)
        {
            element.OnConnect.RemoveListener(TargetConnected);
            elementInRange.Remove(element);
            if (targetElement == element)
            {
                targetElement = null;
                if (TargetChange != null)
                {
                    TargetChange();
                }
            }
        }




    }
}

