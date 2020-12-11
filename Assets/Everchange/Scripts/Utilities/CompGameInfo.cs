﻿using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// Unity Component for the GameInfo class.
    /// </summary>
    public class CompGameInfo : MonoBehaviour
    {
        // UNITY EDITOR
        [Header("General")]
        [TextArea(1, 1)]
        public string __itemName = "";
        [TextArea(1, 2)]
        public string __itemDescription = "";
        [Header("Vendor")]
        public bool __saleable = false;
        [Min(0)]
        public int __itemPrice;
        // ACCESSORS
        private GameInfo Info { get; set; }

        protected virtual void Awake()
        {
            this.Info = new GameInfo(
                this.__itemName, 
                this.__itemDescription, 
                this.__itemPrice,
                this.__saleable);
        }

        /// <summary>
        /// Data about this item that will be used displayed
        /// to the user via UI.
        /// </summary>
        public GameInfo GetGameInfo() => Info;
    }
}
