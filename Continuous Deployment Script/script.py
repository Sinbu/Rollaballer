import requests
import json
import tinys3
import zipfile
import StringIO
import os

def lambda_handler(event, context):
	conn = tinys3.Connection("AKIAJTDGI6SEZTFNVAQQ","vGqdq36MMSqzpRzxadRglWICu4LWymuYLMTnOq8K")
	tokenId = "Basic 87d910883bc9446ed7922880efd21836"
	print event
	buildLink = event["links"]["api_self"]["href"]
	# buildLink = "/api/orgs/sina-yeganeh/projects/roller-baller/buildtargets/webgl-build/builds/2"

	authPayload = {"Authorization": tokenId}
	buildData = requests.get("https://build-api.cloud.unity3d.com" + buildLink, headers=authPayload)
	primaryLink = json.loads(buildData.text)["links"]["download_primary"]["href"]
	print primaryLink
	results = requests.get(primaryLink)
	zip = zipfile.ZipFile(StringIO.StringIO(results.content))
	zip.extractall("./tmp/")

	f = open("./tmp/WebGL build/index.html",'rb')
	conn.upload('index.html',f,'rollerballer')

	files = os.listdir("./tmp/WebGL build/Build")
	for filename in files:
		f = open("./tmp/WebGL build/Build/" + filename,'rb')
		conn.upload("Build/" + filename,f,'rollerballer')
	files = os.listdir("tmp/WebGL build/TemplateData")
	for filename in files:
		f = open("./tmp/WebGL build/TemplateData/" + filename,'rb')
		conn.upload("TemplateData/" + filename,f,'rollerballer')

	return "Done"