.class public Lutube/NullCheck;
.super Ljava/lang/Object;
.source "NullCheck.java"


# direct methods
.method public constructor <init>()V
    .locals 0

    .line 5
    invoke-direct {p0}, Ljava/lang/Object;-><init>()V

    return-void
.end method

.method public static ensureHasFragment(Ljava/lang/String;)Ljava/lang/String;
    .locals 1
    .param p0, "fragmentName"    # Ljava/lang/String;

    .line 7
    invoke-static {p0}, Landroid/text/TextUtils;->isEmpty(Ljava/lang/CharSequence;)Z

    move-result v0

    if-eqz v0, :cond_0

    const-string v0, "placeholder"

    return-object v0

    .line 8
    :cond_0
    return-object p0
.end method
