public class StorageManager{
	private DataContextLoaDB context;
	
	public DataContextLoaDB StorageManager()
	{
		context = new DataContextLoaDB();
	}
	
	public void save() {}
    public string getEnvValue(string envName) { }
    public bool setEnvValue(string envName, string newValue) { }

    public int login(string username, int serviceID) { }
	public void removeUser(int userID) {}
	public User getUser(int userID) {}
		
	public ExternalAccount addExternalAccount(int userID, string username, int serviceID){}
	public void removeExternalAccount(int externalAccountID){}
	public ExternalAccount getExternalAccount(int externalAccountID){}
	
	public ExternalService addService(string newExternalService){}
	public void removeService(int serviceID){}
	public ExternalService getService(int serviceID){}
	
	public Goup addGroup(int userID, string nameGroup){}
	public void removeGroup(int groupID){}
	public Group getGroup(int groupID){}
	
	public Contact saveContact(int userID, string name, string email){}
	public void removeContact(int contactID){}
	public Contact getContact(int contactID){}
	
	public void addContactToGroup(int contactID, int groupID){}
	public void removeContactToGroup(int contactID, int groupID){}
	
	/*
	themeID è l'id del thema, se il modello non ha un tema associato il parametro passato deve essere null
	e noi gli associamo il tema di default
	*/
	public Model saveModel(int userID, int themeID, string nameModel, string xml){}
	public void deleteModel(int modelID){}
	public Model getModel(int modelID){}
	
	public Theme addTheme(int userID, string nameTheme, string css, Image logo){}
	public void removeTheme(int themeID){}
	public Theme getTheme(int themeID){}
	
	/*
	se isPublic è true inseriamo su CompilationRequest un record con token = null e contactID = utentepubblico,
	invece se è false i vari compilation request verranno inseriti invocando addContactToPublication
	*/
	public Publication publish(int modelID, System.DateTime date, string urlBase, bool isPublic){}
	public void removePublication(int publicatonID){}		//cancella a cascata i suoi risultati
	public Publication getPublication(int publicatonID){}
	
	public void addContactToPublication(int publicationID, int contactID, string token){}
	public void removeContactToPublication(int compilationRequestID){}
	
	public Result addResult(int compilationRequestID, string xmlResult){}
	public List<int> getResultID(int compilationRequest){}
    public List<System.XmlElement> getResultXML(int compilationRequest) { }
	
	/*
	categoryID è l'id della categoria, se il field non ha una categoria associata il parametro passato deve essere null
	e noi gli associamo la categoria di default
	*/
	public Field saveField(int userID, int categoryID, string name, string xml){}
	public void removeField(int fieldID){}
	public Field getField(int fieldID){}
	
	public Category addCategory(string name){}
	public void removeCategory(int categoryID){}
	public Category getCategory(int categoryID){}
	
	public void addFieldToCategory(int fieldID, int categoryID){}
	public void removeFieldToCategory(int fieldID) {}
    public void changeFieldToCategory(int fieldID, int categoryID) { }
	
	public FileUploaded uploadFile(int directoryID, string name){}
	public void removeFile(int fileID){}
	public FIleUploaded getFile(int fileID){}

	public Directory addDirectory(int userID, string nameDirectory, int parentDirectoryID){}
	public void removeDirectory(int directoryID){}
	public Directory getDirectory(int directoryID){}
	
	public void moveFileToDirectory(int fileID, int newDirectoryID){}
	public void removeFileFromDirectory(int fileID, int directoryID){}	//la mette nella directory di default	
	
}
