# -*- coding: utf-8 -*-
# from scrapy.spiders import CrawlSpider, Rule
# from scrapy.linkextractors import LinkExtractor
import os
import csv
import glob
import MySQLdb
from scrapy import Spider
from scrapy.http import Request


def product_description(response, value):
    return response.xpath ('//th[text()="{}"]/following-sibling::td/text()'.format(value)).extract_first()


class BooksSpider(Spider):
    name = 'booksold'
    allowed_domains = ['books.toscrape.com']
    start_urls = ['http://books.toscrape.com']
    # def start_requests(self):
    #     self.driver = webdriver.Firefox('/home/tumnus/Projects/geckodriver')
    #     self.driver.get('https://books.toscrape.com')

    #     sel = Selector(text=self.driver.page_source)
    #     books = sel.xpath('//h3/a/@href').extract()
    #     for book in books:
    #         url = 'http://books.toscrape.com/' + book
    #         yield Request(url, callback=self.parse_book)
    #     while True:
    #         try:
    #             next_page = self.driver.find_element_by_xpath('//a[text()="next"]')
    #             sleep(3)
    #             self.logger.info('Wait three minuts')
    #             next_page.click()

    #             sel = Selector(text=self.driver.page_source)
    #             books = sel.xpath('//h3/a/@href').extract()
    #             for book in books:
    #                 url = 'http://books.toscrape.com/catalogue/' + book
    #                 yield Request(url, callback=self.parse_book)
    #         except NoSuchElementException:
    #             self.logger.info('No more pages to load.')
    #             self.driver.quit()
    #             break
    # def __init__(self, category):
    #     self.start_urls = [category]

    def parse(self, response):
        books = response.xpath('//h3/a/@href').extract()
        for book in books:
            absolute_url = response.urljoin(book)
            yield Request(absolute_url, callback=self.parse_book)

        # next_page_url = response.xpath('//a[text()="next"]/@href').extract_first()
        # absolute_next_url = response.urljoin(next_page_url)
        # yield Request(absolute_next_url)
    
    def parse_book(self, response):
        title = response.css('h1::text').extract_first()
        price = response.xpath('//*[@class="price_color"]/text()').extract_first()
        image_url = response.xpath('//img/@src').extract_first()
        image_url = image_url.replace('../..', 'http://books.toscrape.com')

        rating = response.xpath('//*[contains(@class, "star-rating")]/@class').extract_first()
        rating = rating.replace('star-rating ', '')

        description = response.xpath('//*[@id="product_description"]/following-sibling::p/text()').extract_first()
       
        upc = product_description(response, 'UPC')
        product_type = product_description(response, 'Product Type')
        price_without_tax = product_description(response, 'Price (excl. tax)')
        price_with_tax = product_description(response, 'Price (incl. tax)')
        tax = product_description(response, 'Tax')
        availability = product_description(response, 'Availability')
        number_of_reviews = product_description(response, 'Number of reviews')
        yield {
            'title': title,
            'rating': rating,
            'upc': upc,
            'product_type': product_type
        }
        # yield {
        #     'title': title,
        #     'price': price,
        #     'image_url': image_url,
        #     'rating': rating,
        #     'description': description,
        #     'upc': upc,
        #     'product_type': product_type,
        #     'price_without_tax': price_without_tax,
        #     'price_with_tax': price_with_tax,
        #     'tax': tax,
        #     'availability': availability,
        #     'number_of_reviews': number_of_reviews
        # }
    # def close(self, reason):
    #     csv_file = max(glob.iglob('*.csv'), key=os.path.getctime)
    #     os.rename(csv_file, 'foobar.csv')
    def close(self, reason):
        csv_file = max(glob.iglob('*.csv'), key=os.path.getctime)
        print("DUPA")
        print(csv_file)

        mydb = MySQLdb.connect(host='localhost', user='root', passwd='KryptografiaSHA2', db='books_db')

        cursor = mydb.cursor()
        csv_data = csv.reader(open(csv_file))

        row_count = 0
        for row in csv_data:
            if row_count != 0:
                print(row)
                cursor.execute('INSERT INTO books_table(title, rating, upc, product_type) VALUES(%s, %s, %s, %s)', (row[0][0:19],row[1],row[2],row[3]))
            row_count += 1
        mydb.commit()
        cursor.close()