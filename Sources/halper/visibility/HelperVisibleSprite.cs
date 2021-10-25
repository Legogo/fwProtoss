using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace halper.visibility
{

    public class HelperVisibleSprite : HelperVisible
    {
        SpriteRenderer _spriteRenderDefault;
        SpriteRenderer[] _spriteRenders;

        public HelperVisibleSprite(MonoBehaviour parent) : base(parent.transform, parent)
        { }

        public HelperVisibleSprite(Transform pivot) : base(pivot, null)
        { }

        protected override void fetchRenders()
        {
            _spriteRenderDefault = _t.GetComponent<SpriteRenderer>();
            if (_spriteRenderDefault == null) _spriteRenderDefault = _t.GetComponentInChildren<SpriteRenderer>();
            _spriteRenders = HalperComponentsGenerics.getComponents<SpriteRenderer>(_t);

            Debug.Log(_t.name + " has x" + _spriteRenders.Length + " sprites", _t);
        }

        protected override Transform fetchCarrySymbol()
        {
            //if (_renderSprite == null) Debug.LogError(_owner.GetType()+" no render sprite for " + _owner.name, _owner.gameObject);
            if (_spriteRenderDefault == null) return null;
            return _spriteRenderDefault.transform;
        }

        public Sprite getSprite() { return _spriteRenderDefault.sprite; }

        public void setSprite(Sprite newSprite)
        {
            if (_spriteRenderDefault == null)
            {
                Debug.LogWarning(GetType() + " trying to assign a sprite to " + _t.name + " but no renderer found", _t);
                return;
            }

            _spriteRenderDefault.sprite = newSprite;
        }

        override public Color getColor()
        {
            return _spriteRenderDefault.color;
        }

        override protected void swapColor(Color col)
        {
            _spriteRenderDefault.color = col;
        }

        override public void setVisibility(bool flag)
        {
            if (_spriteRenderDefault == null)
            {
                Debug.LogWarning("no render sprite for " + _coroutineCarrier.name, _coroutineCarrier.gameObject);
                return;
            }
            _spriteRenderDefault.enabled = flag;
        }

        /* dans le cas d'une animation il vaut mieux inverser le scale plutôt que de taper sur le flip sprite */
        public override void flipHorizontal(int dir)
        {
            //base.flipHorizontal(dir);
            Vector3 flipScale = _spriteRenderDefault.transform.localScale;
            flipScale.x = Mathf.Abs(flipScale.x) * Mathf.Sign(dir);
            _spriteRenderDefault.transform.localScale = flipScale;

            // _render.flipX = dir < 0;
        }

        override public bool isVisible()
        {
            if (_spriteRenderDefault != null) return _spriteRenderDefault.enabled;
            return false;
        }

        public override Bounds getSymbolBounds()
        {
            if (_spriteRenderDefault == null) Debug.LogWarning("no render sprite for <b>" + _coroutineCarrier.name + "</b>", _coroutineCarrier);
            return _spriteRenderDefault.bounds;
        }

        public SpriteRenderer getSprRender()
        {
            return _spriteRenderDefault;
        }

        public int getZOrder()
        {
            return (int)(_spriteRenderDefault.bounds.min.y * -100f);
            //return (int)(_t.position.y * -100f);
        }

        /// <summary>
        /// uses bottom coord of sprite
        /// </summary>
        static public void setZOrderByYPositioning(SpriteRenderer render)
        {
            render.sortingOrder = (int)(render.bounds.min.y * -100f);
        }

        public void setZOrder() => setZOrder(getZOrder());
        public void setZOrder(int order)
        {
            if (_spriteRenders.Length <= 0) Debug.LogWarning("no renderS ?");

            //Debug.Log(order);

            for (int i = 0; i < _spriteRenders.Length; i++)
            {
                //_spriteRenders[i].renderingLayerMask = (uint)order;
                _spriteRenders[i].sortingOrder = order;
                //_spriteRenders[i].sortingLayerID = order;
                //_spriteRenders[i].renderingLayerMask = (uint)order;
            }
        }

    }

}
