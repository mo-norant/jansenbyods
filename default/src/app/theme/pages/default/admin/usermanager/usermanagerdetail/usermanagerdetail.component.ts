import { ConfirmationService } from 'primeng/api';
import { AdminService } from './../../../../../../_services/admin.service';
import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { ToastrService } from '../../../../../../../../node_modules/ngx-toastr';

@Component({
  selector: 'app-usermanagerdetail',
  templateUrl: './usermanagerdetail.component.html',
  styles: []
})
export class UsermanagerdetailComponent implements OnInit {

  user;
  loading: boolean;
  message: string;
  subject: string;
  editorConfig = {
    "editable": true,
    "spellcheck": true,
    "height": "300",
    "minHeight": "300",
    "width": "auto",
    "minWidth": "0",
    "translate": "yes",
    "enableToolbar": true,
    "showToolbar": true,
    "placeholder": "Bericht versturen",
    "imageEndPoint": "",
    "toolbar": [
      ["bold", "italic", "underline", "strikeThrough", "superscript", "subscript"],
      ["fontName", "fontSize", "color"],
      ["justifyLeft", "justifyCenter", "justifyRight", "justifyFull", "indent", "outdent"],
      ["cut", "copy", "delete", "removeFormat", "undo", "redo"],
      ["paragraph", "blockquote", "removeBlockquote", "horizontalLine", "orderedList", "unorderedList"],
      ["link", "unlink", "image", "video"]
    ]
  }

  constructor(private router: Router, private route: ActivatedRoute, private adminservice: AdminService, private toastr: ToastrService,
    private dialogservice: ConfirmationService
  ) { }

  ngOnInit() {
    this.loading = true;
    this.route.params.subscribe(res => {
      //verstuurd met base64 encodering
      this.adminservice.GetUser(res.id).subscribe(res => {
        this.loading = false;
        res["createDate"] = new Date(res["createDate"]);
        this.user = res;
        this.showSuccess(`Gebruiker ${this.user.userName} werd geladen `, "success");
      }, err => {
        this.showSuccess("Gebruiker werd niet geladen", "warning");
        this.loading = false;
        this.router.navigate(['admin/usermanager'])

      });
    });
  }

  delete() {
    this.dialogservice.confirm({
      message: "Wilt u deze gebruiker verwijderen, met al zijn producten en aanvragen?",
      accept: () => {
        this.loading = true;
        this.adminservice.DeleteUser(this.user.id).subscribe(res => {
          this.loading = false;
          this.showSuccess(`Gebruiker ${this.user.userName} werd verwijderd `, "danger");
          this.router.navigate(['admin/usermanager']);
        }, err => {
          this.showSuccess(`Gebruiker ${this.user.userName} werd niet verwijderd `, "danger");
          this.loading = false;

        })

      }
    });
  }

  sendMessage() {
    this.dialogservice.confirm({
      message: "Wilt u een bericht versturen?",
      accept: () => {
        this.loading = true;
        this.adminservice.PostMessage(this.user.id, this.message, this.subject).subscribe(res => {
          this.loading = false;
          this.showSuccess('Bericht werd verstuurd', "success");
          this.message = "";
          this.subject = "";
        }, err => {
          this.showSuccess('Bericht werd niet verstuurd', "danger");
          this.loading = false;

        })

      }
    });
  }

  lockenable(){

    if(!this.user.lockoutEnabled){
      this.dialogservice.confirm({
        message: "Wilt u deze gebruiken blokkeren?",
        accept: () => {
          this.loading = true;
          this.adminservice.lockEnable(this.user.id).subscribe(res => {
            this.user.lockoutEnabled = res;
            this.loading = false;
            this.showSuccess('Gebruikersstatus werd aangepast', "success");
    
          }, err => {
           this.showSuccess('Gebruikersstatus werd niet aangepast.', "danger");
           this.loading = false;
          });
  
        }
      });
    }
    else{

      this.dialogservice.confirm({
        message: "Wilt u deze gebruiken deblokkeren?",
        accept: () => {
          this.loading = true;
          this.adminservice.lockEnable(this.user.id).subscribe(res => {
            this.user.lockoutEnabled = res;
            this.loading = false;
            this.showSuccess('Gebruikersstatus werd aangepast', "success");
    
          }, err => {
           this.showSuccess('Gebruikersstatus werd niet aangepast.', "danger");
           this.loading = false;
          });
        }
      });
    
    }

  }

  resetPassword() {
    this.dialogservice.confirm({
      message: "Gebruiker krijgt een email waar hij zijn wachtwoord kan resetten.",
      accept: () => {
        this.loading = true;
        this.adminservice.resetPassword(this.user.userName).subscribe(res => {
          this.loading = false;
          this.showSuccess('Gebruiker kreeg een email waarmee hij zijn wachtwoord kan resetten.', "success");

        }, err =>{
          this.showSuccess('Wachtwoord werd niet gereset', "danger");
          this.loading = false;
        });
      }
    })
  };



  cancel() {
    this.router.navigate(['admin/usermanager'])
  }

  showSuccess(message: string, serviraty: string) {
    if (serviraty === "success") {
      this.toastr.success(message);
    }
    else {
      this.toastr.warning(message);
    }
  }


}
