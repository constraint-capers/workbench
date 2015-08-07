﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;

namespace DynaApp.ViewModels
{
    /// <summary>
    /// Base for all graphic elements displayed on the model view.
    /// </summary>
    public abstract class GraphicViewModel : AbstractViewModel
    {
        /// <summary>
        /// The X coordinate for the position of the graphic.
        /// </summary>
        private double x;

        /// <summary>
        /// The Y coordinate for the position of the graphic.
        /// </summary>
        private double y;

        /// <summary>
        /// Set to 'true' when the graphic is selected.
        /// </summary>
        private bool isSelected;

        /// <summary>
        /// The graphic name.
        /// </summary>
        private string name;

        /// <summary>
        /// Initialize a graphic with a name and location.
        /// </summary>
        protected GraphicViewModel(string newName, Point newLocation)
            : this(newName)
        {
            this.X = newLocation.X;
            this.Y = newLocation.Y;
        }

        /// <summary>
        /// Initialize a graphic with a name.
        /// </summary>
        /// <param name="newName"></param>
        protected GraphicViewModel(string newName)
            : this()
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("newName");
            this.Name = newName;
        }

        /// <summary>
        /// Initialize a graphic with default values.
        /// </summary>
        protected GraphicViewModel()
        {
            this.Name = string.Empty;
            this.Connectors = new ObservableCollection<ConnectorViewModel>();
            this.Connectors.CollectionChanged += connectors_CollectionChanged;
        }

        /// <summary>
        /// Gets or sets the domain name.
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set
            {
                if (this.name == value) return;
                this.name = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the connectors (connection anchor points) attached to the graphic.
        /// </summary>
        public ObservableCollection<ConnectorViewModel> Connectors { get; private set; }

        /// <summary>
        /// The X coordinate for the position of the domain.
        /// </summary>
        public double X
        {
            get
            {
                return x;
            }
            set
            {
                if (x.Equals(value)) return;
                x = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The Y coordinate for the position of the domain.
        /// </summary>
        public double Y
        {
            get
            {
                return y;
            }
            set
            {
                if (y.Equals(value)) return;
                y = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets selected status of the domain.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                if (isSelected == value) return;
                isSelected = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets all connections attached to the graphic.
        /// </summary>
        public ICollection<ConnectionViewModel> AttachedConnections
        {
            get
            {
                var attachedConnections = new List<ConnectionViewModel>();

                foreach (var connector in this.Connectors)
                {
                    if (connector.AttachedConnection != null)
                    {
                        attachedConnections.Add(connector.AttachedConnection);
                    }
                }

                return attachedConnections;
            }
        }

        /// <summary>
        /// Is the destination graphic connectable to this graphic?
        /// </summary>
        /// <param name="destinationGraphic">Destination being connected to.</param>
        /// <returns>True if the destination can be connected, False if it cannot be connected.</returns>
        public virtual bool IsConnectableTo(GraphicViewModel destinationGraphic)
        {
            return false;
        }

        private void connectors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (ConnectorViewModel connector in e.NewItems)
                connector.Parent = this;
        }
    }
}