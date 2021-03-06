using AutoMapper;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.App_Start
{
    public class AutoMapperConfig
    {
        public static void Register()
        {
            Mapper.Initialize(cfg => {
                //cfg.CreateMap<DAL.Order, BasketApi.AppsViewModels.OrderViewModel>();
                //cfg.CreateMap<DAL.StoreOrder, BasketApi.AppsViewModels.StoreOrderViewModel>();
                cfg.CreateMap<DAL.UserSubscriptions, BasketApi.AdminViewModel.SubscriptionListViewModel>();
                cfg.CreateMap<LoginResponseModel, User>().ReverseMap();
            });
        }
    }
}