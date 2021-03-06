﻿using DG.Tweening;
using FairyGUI.Utils;

namespace FairyGUI
{
	/// <summary>
	/// Gear is a connection between object and controller.
	/// </summary>
	abstract public class GearBase
	{
		public static bool disableAllTweenEffect = false;

		/// <summary>
		/// Pages involed in this gear.
		/// </summary>
		public PageOptionSet pageSet { get; private set; }

		/// <summary>
		/// Use tween to apply change.
		/// </summary>
		public bool tween;

		/// <summary>
		/// Ease type.
		/// </summary>
		public Ease easeType;

		/// <summary>
		/// Tween duration in seconds.
		/// </summary>
		public float tweenTime;

		/// <summary>
		/// Tween delay in seconds.
		/// </summary>
		public float delay;

		protected GObject _owner;
		protected Controller _controller;

		protected static char[] jointChar0 = new char[] { ',' };
		protected static char[] jointChar1 = new char[] { '|' };

		public GearBase(GObject owner)
		{
			_owner = owner;
			pageSet = new PageOptionSet();
			easeType = Ease.OutQuad;
			tweenTime = 0.3f;
			delay = 0;
		}

		/// <summary>
		/// Controller object.
		/// </summary>
		public Controller controller
		{
			get
			{
				return _controller;
			}

			set
			{
				if (value != _controller)
				{
					_controller = value;
					pageSet.controller = value;
					pageSet.Clear();
					if (_controller != null)
						Init();
				}
			}
		}

		public void Setup(XML xml)
		{
			string str;

			_controller = _owner.parent.GetController(xml.GetAttribute("controller"));
			if (_controller == null)
				return;

			Init();

			string[] pages = xml.GetAttributeArray("pages");
			if (pages != null)
			{
				foreach (string s in pages)
					pageSet.AddById(s);
			}

			str = xml.GetAttribute("tween");
			if (str != null)
				tween = true;

			str = xml.GetAttribute("ease");
			if (str != null)
				easeType = FieldTypes.ParseEaseType(str);

			str = xml.GetAttribute("duration");
			if (str != null)
				tweenTime = float.Parse(str);

			str = xml.GetAttribute("delay");
			if (str != null)
				delay = float.Parse(str);

			str = xml.GetAttribute("values");
			string[] values = null;
			if (str != null)
				values = str.Split(jointChar1);

			if (pages != null && values != null)
			{
				for (int i = 0; i < values.Length; i++)
				{
					str = values[i];
					if (str != "-")
						AddStatus(pages[i], str);
				}
			}
			str = xml.GetAttribute("default");
			if (str != null)
				AddStatus(null, str);
		}

		virtual protected bool connected
		{
			get
			{
				if (_controller != null && !pageSet.isEmpty)
					return pageSet.ContainsId(_controller.selectedPageId);
				else
					return false;
			}
		}

		abstract protected void AddStatus(string pageId, string value);
		abstract protected void Init();

		/// <summary>
		/// Call when controller active page changed.
		/// </summary>
		abstract public void Apply();

		/// <summary>
		/// Call when object's properties changed.
		/// </summary>
		abstract public void UpdateState();
	}
}
