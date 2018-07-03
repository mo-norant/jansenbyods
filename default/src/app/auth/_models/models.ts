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
  locationID: number;
  latitude: number;
  longtitude: number;
}

export class OogstKaartItem {
  oogstkaartItemID: number;
  createDate: Date;
  localdatestring: string;
  omschrijving: string;
  artikelnaam: string;
  jansenserie: string;
  datumBeschikbaar: Date;
  company: Company;
  location: LocationOogstKaartItem;
  hoeveelheid: number;
  category: string;
  vraagPrijsPerEenheid: number;
  vraagPrijsTotaal: number;
  onlineStatus: boolean;
  concept: string;
  transportInbegrepen: boolean;
  userID: string;
  Views: number;
  avatar: Afbeelding;
  specificaties: Specificatie[];
  gallery: Afbeelding[];
}

export class Afbeelding {
  afbeeldingID: number;
  uri: string;
  create: Date;
  name: string;
  omschrijving: string;
}

export class Specificatie {
  public specificatieSleutel: number;
  public specificatieID: number;
  public specificatieValue: string;
  public specificatieEenheid: string;
  public specificatieOmschrijving: string;
}

export class MenuItem {
  id: string;
  class: string;
  name: string;
  route: string;
  logo: string;
  menusubitems: MenuSubItem[];
  badgenumber: number;
}

export class MenuSubItem {
  id: string;
  class: string;
  name: string;
  route: string;
  logo: string;
}

export class Menu {
  id: string;
  menuitems: MenuItem[];
}

export class Message {
  messageID: number;
  created: Date;
  messageString: string;
  opened: boolean;
}

export class Request {
  requestID: number;
  oogstkaartID: number;
  name: string;
  company: Company;
  status: string;
  create: any;
  userViewed: boolean;
  messages: Message[];
}

export class CreateUser {
  company: Company;
  email: string;
  password: string;
  password2: string;
  name: string;
}
