using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Solitaire.Utils;

public static class PropertyChangedExtensions
{
    // Observable<T> слушает PropertyChanged, эмитит значение свойства
    class PropertyObservable<T> : IObservable<T>
    {
        // объект ViewModel
        private readonly INotifyPropertyChanged _target;
        // информация о конкретном свойстве
        private readonly PropertyInfo _info;

        public PropertyObservable(INotifyPropertyChanged target, PropertyInfo info)
        {
            _target = target;
            _info = info;
        }

        // подписка, которая подписывается на PropertyChanged, отписывается при Dispose()
        class Subscription : IDisposable
        {
            private readonly INotifyPropertyChanged _target;
            private readonly PropertyInfo _info;
            private readonly IObserver<T> _observer;

            public Subscription(INotifyPropertyChanged target, PropertyInfo info, IObserver<T> observer)
            {
                _target = target;
                _info = info;
                _observer = observer;
                _target.PropertyChanged += OnPropertyChanged!;
                // достаем текущее значение свойства у объекта, приводим его к типу T и передаем подписчику
                // вызывается в момент подписки
                _observer.OnNext(((T)_info.GetValue(_target)!));
            }

            private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                // проверяем что изменилось нужное свойство
                // реакция на изменение
                if (e.PropertyName == _info.Name)
                    // отправялем подписчику
                    _observer.OnNext(((T)_info.GetValue(_target)!));
            }

            public void Dispose()
            {
                // убираем обработчик
                _target.PropertyChanged -= OnPropertyChanged!;
            }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            // каждый подписчик получает своб подписку
            return new Subscription(_target, _info, observer);
        }
    }
    
    // принимает объект модели и указание на ее конкретное совйств model.WhenAnyValue(x => x.SomeProperty)
    // превращает изменнеие этого свойства в IObservable<T>
    //
    // TModel - тип модели, TRes - тип совйства
    // where TModel : INotifyPropertyChanged - это гарантия, что у model есть событие PropertyChanged и мы можем на него подписаться

    public static IObservable<TRes> WhenAnyValue<TModel, TRes>(
        this TModel model,
        Expression<Func<TModel, TRes>> expr) where TModel : INotifyPropertyChanged
    {
        var l = (LambdaExpression)expr;
        var ma = (MemberExpression)l.Body;
        // получаем свойство которое нужно отслеживать в дереве выражений
        var prop = (PropertyInfo)ma.Member;
        
        // PropertyObservable<T> превращает свойство объекта в реактивный поток значений
        // было Property Changed - событие, стало IObservable<T> поток данных
        
        // PropertyObservable<T>
        //   - знает объект (model)
        //   - знает конкретное свойство (PropertyInfo)
        //   - умеет подписываться на PropertyChanged
        //   - при подписке создает прослушку этого свойства
        return new PropertyObservable<TRes>(model, prop);
    }
}