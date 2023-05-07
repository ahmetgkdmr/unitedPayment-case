# unitedPayment-case

                                                             -- ÖZET --
                                                                     
1-) Şuan elimde macbook bulunduğu için macbook için visual studio kullandım. Bu sebeple wcf web servis referansını ekleyemedim. ( Visual Studio Cross platform için wcf desteği vermiyor. Araştırdım çözüm bulamadım). Tc kimlik sorgulama servisini yazdım ancak test edemediğim için yorum satırına aldım.

2-) Tüm servislere başarıyla istek atılıp cevap alındı. Ancak payment servisinde hash bilgisi hatalıdır yanıtı dönüyor. Hash fonksiyonu parametrelerini dokümanı inceleyerek doldurdum. 

3-) Customer ve LogHistory adında 2 tablo oluşturdum. Loglama ve müşteri verilerini buraya kaydettim.

4-) Projede swagger kullanıldı. https://unitedpayment.azurewebsites.net/swagger adresinden servisler test edilebilir.
