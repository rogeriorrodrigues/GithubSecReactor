﻿using DesafioWebCsharp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using X.PagedList;//namespace do pagedlist core

namespace DesafioWebCsharp.Controllers
{

    public class CardsController : Controller
    {
        private readonly string ENDPOINT = "https://api.magicthegathering.io/v1/cards";
        private readonly HttpClient httpClient = null;
        private static List<Card> _cards = null;

        public CardsController() { 
        
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ENDPOINT);

        }
        public async Task<IActionResult> Index(int? page)
        {
            //config para o pagged
            var itemsByPage = 10;
            var currentPage = page ?? 1;

            try
            {
                MagicCard cardList = null;
                _cards = new List<Card>();

                HttpResponseMessage response = await httpClient.GetAsync(ENDPOINT);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    cardList = JsonConvert.DeserializeObject<MagicCard>(content);
                    

                    foreach (var card in cardList.cards)
                    { 
                        _cards.Add(card);
                    }

                }
                else
                {
                    ModelState.AddModelError(null, "Erro ao realizar a chamada");
                }
                //Com paginação
                return View(await cardList.cards.ToPagedListAsync(currentPage, itemsByPage));
                //Sem Paginação
                //return View(cardList.cards);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw ex;
            }
           
        }

        //Método para os detalhes
        [HttpGet]
        public IActionResult Details(string id)
        {
            Card card = _cards.FirstOrDefault(c => c.id.Equals(id));
            return View(card);
             
        }
    }
}
