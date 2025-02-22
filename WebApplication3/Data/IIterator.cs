using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication3.Models;

namespace WebApplication3.Data
{
    public interface IIterator
    {
        CartItem First();
        CartItem Next();
        bool IsDone {  get; }
        CartItem CurrentItem { get; }

    }
    public class CartIIterator : IIterator
    {
        List<CartItem> _listCart;
        int current = 0;
        int step = 1;
        public CartIIterator(List<CartItem> listCart) 
        {
            _listCart = listCart;
        }

        public bool IsDone
        {
            get { return current >= _listCart.Count; }
        }

        public CartItem CurrentItem => _listCart[current];

        public CartItem First()
        {
            current = 0;
            if (_listCart.Count > 0)
            {
                return _listCart[current];
            }
            return null;
        }

        public CartItem Next()
        {
            current += step;
            if (!IsDone)
            {
                return _listCart[current];
            }
            else
            {
                return null;
            }
        }
    }
}