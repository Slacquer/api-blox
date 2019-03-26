
// originally stolen from 
// https://github.com/Azure/azure-documentdb-hadoop/blob/master/src/BulkImportScript.js
// modified to handle updating root document.

function bulkInsert(keyName, keyValue, docs) {
    var collection = getContext().getCollection();
    var collectionLink = collection.getSelfLink();

    // The count of imported docs, also used as current doc index.
    var count = 0;

    if (!docs)
        throw new Error("The docs is undefined or null.");

    if (!keyName)
        throw new Error("The keyName is undefined or null.");

    if (!keyValue)
        throw new Error("The keyValue is undefined or null.");

    var docsLength = docs.length;

    if (docsLength === 0) {
        getContext().getResponse().setBody(0);
        return;
    }

    tryCreateUpdate(docs[count], callback);

    function tryCreateUpdate(doc, cb) {
        doc[keyName] = keyValue;

        var isAccepted;

        if (doc.documentType === "Root") {
            var query = {
                query: "select * from root r where r.id = @id",
                parameters: [{ name: "@id", value: doc.id }]
            };
            var requestOptions = {};
            isAccepted = collection.queryDocuments(collectionLink,
                query,
                requestOptions,
                function (err, retrievedDocs) {
                    if (err)
                        throw err;
                    if (retrievedDocs.length > 0) {
                        isAccepted = collection.replaceDocument(retrievedDocs[0]._self, doc, cb);
                    } else {
                        isAccepted = collection.createDocument(collectionLink, doc, cb);
                    }
                });
        } else
        isAccepted = collection.createDocument(collectionLink, doc, cb);

        if (!isAccepted)
            getContext().getResponse().setBody(count);
    }

    function callback(err) {
        if (err)
            throw err;

        count++;

        if (count >= docsLength) {
            getContext().getResponse().setBody(count);
        } else {
            tryCreateUpdate(docs[count], callback);
        }
    }
}