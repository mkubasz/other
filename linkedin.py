import csv
import parameters
from parsel import Selector
from time import sleep
from selenium import webdriver
from selenium.webdriver.common.keys import Keys

writer = csv.writer(open(parameters.file_name, 'w'))
writer.writerow(['Name', 
                 'Job Title', 
                 'Company', 
                 'School', 
                 'Location', 
                 'URL'])


driver = webdriver.Firefox('/home/tumnus/Projects/geckodriver')
driver.get('https://www.linkedin.com')

username = driver.find_element_by_class_name('login-email')
username.send_keys(parameters.linkedin_username)
sleep(0.5)

password = driver.find_element_by_id('login-password')
password.send_keys(parameters.linkedin_password)
sleep(0.5)

sign_in_btn = driver.find_element_by_xpath('//*[@type="submit"]')
sign_in_btn.click()
sleep(5)

driver.get('https://www.google.com')

search_query = driver.find_element_by_name('q')
search_query.send_keys(parameters.search_query)
sleep(0.5)

search_query.send_keys(Keys.RETURN)
sleep(3)

linkedin_urls = driver.find_elements_by_tag_name('cite')
linkedin_urls = [ url.text for url in linkedin_urls]
sleep(0.5)

for linkedin_url in linkedin_urls:
    driver.get(linkedin_url)
    sleep(5)

    sel = Selector(text=driver.page_source)
    name = sel.xpath('//h1/text()').extract_first()
    job_title = sel.xpath('//h2/text()').extract_first()
    company = sel.xpath('//*[starts-with(@class, "pv-top-card-section__company")]'/text()).extract_first()
    school = sel.xpath('//*[starts-with(@class, "pv-top-card-section__school")]/text()').extract_first()
    if school:
        school = school.strip()
    location = sel.xpath('//*[starts-with(@class, "pv-top-card-section__location")]/text()').extract_first()
    linkedin_url =  driver.current_url

    print('Name: {0} \n Job title: {1} \n Company: {2} \n School: {3} \n Location: {4} \n URL: {5}'.format(
        name, job_title, company, school, location ,linkedin_url))
    writer.writerow([name, job_title, company, school, location, linkedin_url])
driver.quit()