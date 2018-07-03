
    export class Address {
        addressID: number;
        street: string;
        number: string;
        zipcode: string;
        city: string;
        country: string;
    }

    export class Company {
        companyID: number;
        companyName: string;
        createDate: Date;
        phone: string;
        address: Address;
        email: string;
    }

    export class Weight {
        weightID: number;
        weightX: number;
        metric: string;
    }

    export class LocationOogstKaartItem {
        locationID : number
        latitude : number
        longtitude	:number
        }

    export class OogstKaartItem {
        oogstkaartItemID: number;
        createDate: Date;
        omschrijving: string;
        artikelnaam: string;
        jansenserie: string;
        datumBeschikbaar: Date;
        company: Company;
        location: LocationOogstKaartItem;
        hoeveelheid: number;
        category: string
        vraagPrijsPerEenheid: number;
        vraagPrijsTotaal: number;
        onlineStatus: boolean;
        concept: string;
        transportInbegrepen: boolean;
        userID: string;
        Views: number;
        avatar : Afbeelding;
        specificaties: Specificatie[];
        gallery : Afbeelding[];
        files: File[];


    }

    export class File{
      creation : Date;
      fileID: number;
      name: string;
      omschrijving: string;
      uri: string;
    }

    export class Afbeelding {
        afbeeldingID: number;
        uri: string;
        create: Date;
        name: string;
        omschrijving: string;
    }

    export class Specificatie {
        specificatieID: number;
        specificatieSleutel: string;
        specificatieValue: string;
        specificatieEenheid: string;
        specificatieOmschrijving: string;
    }

    export class Message {
      messageID: number;
      created: Date;
      messageString: string;
      opened: boolean;
  }

  export class Request {
      requestID: number;
      name: string;
      company: Company;
      status: string;
      create: Date;
      userViewed: boolean;
      messages: Message[];
  }




